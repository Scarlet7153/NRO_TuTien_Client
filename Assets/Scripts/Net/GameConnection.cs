using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace NRO.Net
{
    public class GameConnection
    {
        private enum State
        {
            Disconnected,
            Connecting,
            Connected
        }

        private volatile State _state = State.Disconnected;
        private TcpClient _tcpClient;
        private NetworkStream _stream;
        
        private Thread _connectThread;
        private Thread _readThread;
        private Thread _writeThread;

        private readonly object _sendQueueLock = new object();
        private readonly Queue<RawPacket> _sendQueue = new Queue<RawPacket>();

        private readonly ConcurrentQueue<NetEvent> _inboundQueue = new ConcurrentQueue<NetEvent>();
        
        private readonly StreamCipher _cipher = new StreamCipher();
        private volatile bool _keyReady = false;
        
        private int _generation;
        private int _timeConnected;
        
        private long _sendBytes;
        private long _recvBytes;
        
        private readonly INetLog _log;
        private readonly HashSet<sbyte> _bigCmds;

        public GameConnection(INetLog log, HashSet<sbyte> bigCmds)
        {
            _log = log;
            _bigCmds = bigCmds;
        }

        public long SendBytes => Interlocked.Read(ref _sendBytes);
        public long RecvBytes => Interlocked.Read(ref _recvBytes);
        
        public void Connect(string host, int port)
        {
            Close(); // Ensure everything is cleaned up before reconnecting
            
            _state = State.Connecting;
            _generation++;
            int currentGen = _generation;
            
            _connectThread = new Thread(() => ConnectLoop(host, port, currentGen))
            {
                Name = "NetConnect",
                IsBackground = true
            };
            _connectThread.Start();
        }

        private void ConnectLoop(string host, int port, int gen)
        {
            try
            {
                _tcpClient = new TcpClient();
                _tcpClient.NoDelay = true;
                _tcpClient.Connect(host, port);
                
                if (_state != State.Connecting || _generation != gen)
                {
                    _tcpClient.Close();
                    return;
                }

                _stream = _tcpClient.GetStream();
                _timeConnected = Environment.TickCount;
                _state = State.Connected;
                
                // Reset cipher before starting threads (fixes race #6)
                _keyReady = false;
                _cipher.Reset();

                _inboundQueue.Enqueue(new NetEvent { Type = NetEventType.Connected, Generation = gen });

                _readThread = new Thread(() => ReadLoop(gen)) { Name = "NetRead", IsBackground = true };
                _writeThread = new Thread(() => WriteLoop(gen)) { Name = "NetWrite", IsBackground = true };
                
                _readThread.Start();
                _writeThread.Start();

                // Automatically send handshake request
                SendMessage(new RawPacket(-27, null));
            }
            catch (Exception ex)
            {
                if (ex is SocketException se && se.ErrorCode == 10004)
                {
                    // Canceled
                    return;
                }
                
                if (_state == State.Connecting && _generation == gen)
                {
                    _inboundQueue.Enqueue(new NetEvent { Type = NetEventType.ConnectFailed, Generation = gen });
                    _state = State.Disconnected;
                }
            }
        }

        private void ReadLoop(int gen)
        {
            try
            {
                while (_state == State.Connected && _generation == gen)
                {
                    RawPacket? packet = FrameCodec.Decode(_stream, _cipher, _keyReady, _bigCmds);
                    if (!packet.HasValue)
                    {
                        break; // EOF
                    }

                    int bytesAdded = 5 + (packet.Value.Payload != null ? packet.Value.Payload.Length : 0);
                    Interlocked.Add(ref _recvBytes, bytesAdded);

                    if (packet.Value.Command == -27)
                    {
                        if (packet.Value.Payload != null && packet.Value.Payload.Length > 0)
                        {
                            int keyLen = packet.Value.Payload[0];
                            if (keyLen > 0 && packet.Value.Payload.Length >= keyLen + 1)
                            {
                                sbyte[] keyRaw = new sbyte[keyLen];
                                Array.Copy(packet.Value.Payload, 1, keyRaw, 0, keyLen);
                                _cipher.SetKey(keyRaw);
                                _keyReady = true;
                            }
                        }
                        _inboundQueue.Enqueue(new NetEvent { Type = NetEventType.Handshake, Packet = packet.Value, Generation = gen });
                    }
                    else
                    {
                        _inboundQueue.Enqueue(new NetEvent { Type = NetEventType.Message, Packet = packet.Value, Generation = gen });
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is IOException || ex is ObjectDisposedException)
                {
                    // Expected on close
                }
                else
                {
                    _log?.LogError("ReadLoop exception: " + ex.ToString());
                }
            }
            finally
            {
                HandleDisconnect(gen);
            }
        }

        private void WriteLoop(int gen)
        {
            byte[] scratch = new byte[FrameCodec.MaxFrameBytes + 16];
            try
            {
                while (_state == State.Connected && _generation == gen)
                {
                    RawPacket packet;
                    lock (_sendQueueLock)
                    {
                        while (_sendQueue.Count == 0)
                        {
                            if (_state != State.Connected || _generation != gen) return;
                            Monitor.Wait(_sendQueueLock);
                        }
                        packet = _sendQueue.Dequeue();
                    }

                    int len;
                    FrameCodec.Encode(packet, _cipher, _keyReady, scratch, _stream, out len);
                    
                    int bytesAdded = packet.Payload != null ? (5 + packet.Payload.Length) : 5;
                    Interlocked.Add(ref _sendBytes, bytesAdded);
                }
            }
            catch (Exception ex)
            {
                if (ex is IOException || ex is ObjectDisposedException)
                {
                    // Expected on close
                }
                else
                {
                    _log?.LogError("WriteLoop exception: " + ex.ToString());
                }
            }
            finally
            {
                HandleDisconnect(gen);
            }
        }

        private void HandleDisconnect(int gen)
        {
            if (_state == State.Connected && _generation == gen)
            {
                _state = State.Disconnected;
                int duration = Environment.TickCount - _timeConnected;
                
                if (duration <= 500)
                {
                    _inboundQueue.Enqueue(new NetEvent { Type = NetEventType.ConnectFailed, Generation = gen });
                }
                else
                {
                    _inboundQueue.Enqueue(new NetEvent { Type = NetEventType.Disconnected, Generation = gen });
                }
                
                CloseInternal();
            }
        }

        public void SendMessage(RawPacket packet)
        {
            if (_state != State.Connected) return;
            
            lock (_sendQueueLock)
            {
                _sendQueue.Enqueue(packet);
                Monitor.Pulse(_sendQueueLock);
            }
        }

        public void ClearSendingMessage()
        {
            lock (_sendQueueLock)
            {
                _sendQueue.Clear();
            }
        }

        public bool TryGetEvent(out NetEvent ev)
        {
            while (_inboundQueue.TryDequeue(out ev))
            {
                if (ev.Generation == _generation)
                {
                    return true;
                }
                // Skip events from old generations
            }
            return false;
        }

        public void Close()
        {
            _generation++; // Invalidate pending operations
            _state = State.Disconnected;
            CloseInternal();
            
            // Wait for threads to terminate gracefully if we are not on one of them
            if (Thread.CurrentThread != _readThread && Thread.CurrentThread != _writeThread && Thread.CurrentThread != _connectThread)
            {
                _connectThread?.Join(100);
                _readThread?.Join(200);
                _writeThread?.Join(200);
            }
        }

        private void CloseInternal()
        {
            if (_stream != null)
            {
                try { _stream.Close(); } catch { }
                _stream = null;
            }
            if (_tcpClient != null)
            {
                try { _tcpClient.Close(); } catch { }
                _tcpClient = null;
            }
            
            lock (_sendQueueLock)
            {
                Monitor.PulseAll(_sendQueueLock);
            }
        }
        
        public bool IsConnected => _state == State.Connected;
        public bool IsConnecting => _state == State.Connecting;
    }
}
