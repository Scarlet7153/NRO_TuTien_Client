using System;
using System.Collections.Generic;
using NRO.Net;
using NRO.Util;

public class Session_ME : ISession, INetLog
{
    protected static Session_ME instance = new Session_ME();

    public static IMessageHandler messageHandler;
    public static bool isMainSession = true;

    private GameConnection _conn;
    private Queue<NetEvent> _localEventQueue = new Queue<NetEvent>();

    [Obsolete("Use _conn.SendBytes instead")]
    public static int sendByteCount => instance != null && instance._conn != null ? (int)instance._conn.SendBytes : 0;
    
    [Obsolete("Use _conn.RecvBytes instead")]
    public static int recvByteCount => instance != null && instance._conn != null ? (int)instance._conn.RecvBytes : 0;

    public static bool connected => instance != null && instance._conn != null && instance._conn.IsConnected;
    public static bool connecting => instance != null && instance._conn != null && instance._conn.IsConnecting;
    public static bool isCancel;
    public static int count;
    public static sbyte[] key = null; 

    public static string strRecvByteCount
    {
        get
        {
            if (instance == null || instance._conn == null) return "0.0Kb";
            long total = instance._conn.RecvBytes + instance._conn.SendBytes;
            return (total / 1024) + "." + ((total % 1024) / 102) + "Kb";
        }
        set { }
    }

    public Session_ME()
    {
        _conn = new GameConnection(this, new HashSet<sbyte> { -32, -66, 11, -67, -74, -87, 66 });
    }

    public static Session_ME gI()
    {
        if (instance == null) instance = new Session_ME();
        return instance;
    }

    public bool isConnected() => _conn != null && _conn.IsConnected;
    
    public void setHandler(IMessageHandler msgHandler)
    {
        messageHandler = msgHandler;
    }

    public void connect(string host, int port)
    {
        if (string.IsNullOrEmpty(host) || host == "127.0.0.0") host = "192.168.2.4";
        if (port <= 0) port = 14445;

        _localEventQueue.Clear();
        _conn.Connect(host, port);
    }

    public void sendMessage(Message message)
    {
        count++;
        sbyte[] data = message.getData();
        _conn.SendMessage(new RawPacket(message.command, data));
    }

    public void clearSendingMessage()
    {
        _conn.ClearSendingMessage();
    }

    public void close()
    {
        _conn.Close();
    }

    public static void update()
    {
        if (instance == null || instance._conn == null) return;

        while (instance._conn.TryGetEvent(out NetEvent ev))
        {
            instance._localEventQueue.Enqueue(ev);
        }

        while (instance._localEventQueue.Count > 0)
        {
            NetEvent ev = instance._localEventQueue.Peek();

            if (ev.Type == NetEventType.Message)
            {
                if (Controller.isStopReadMessage) break;
                
                instance._localEventQueue.Dequeue();
                Message msg = new Message(ev.Packet.Command, ev.Packet.Payload);
                if (messageHandler != null) messageHandler.onMessage(msg);
            }
            else
            {
                instance._localEventQueue.Dequeue();
                if (ev.Type == NetEventType.Connected)
                {
                    if (messageHandler != null) messageHandler.onConnectOK(isMainSession);
                }
                else if (ev.Type == NetEventType.ConnectFailed)
                {
                    if (messageHandler != null) messageHandler.onConnectionFail(isMainSession);
                }
                else if (ev.Type == NetEventType.Disconnected)
                {
                    if (messageHandler != null) messageHandler.onDisconnected(isMainSession);
                }
                else if (ev.Type == NetEventType.Handshake)
                {
                    if (ev.Packet.Payload != null)
                    {
                        try {
                            Message msg = new Message(ev.Packet.Command, ev.Packet.Payload);
                            sbyte b = msg.reader().readSByte();
                            for (int i = 0; i < b; i++) msg.reader().readSByte();
                            
                            GameMidlet.IP2 = msg.reader().readUTF();
                            GameMidlet.PORT2 = msg.reader().readInt();
                            GameMidlet.isConnect2 = (msg.reader().readByte() != 0);
                            
                            if (isMainSession && GameMidlet.isConnect2)
                            {
                                GameCanvas.connect2();
                            }
                        } catch { }
                    }
                }
            }
        }
    }

    public static int currentTimeMillis()
    {
        return Environment.TickCount;
    }

    public static byte convertSbyteToByte(sbyte var)
    {
        if (var > 0) return (byte)var;
        return (byte)(var + 256);
    }

    public static byte[] convertSbyteToByte(sbyte[] var)
    {
        byte[] array = new byte[var.Length];
        for (int i = 0; i < var.Length; i++)
        {
            if (var[i] > 0) array[i] = (byte)var[i];
            else array[i] = (byte)(var[i] + 256);
        }
        return array;
    }

    public bool isCompareIPConnect()
    {
        return true;
    }

    public void LogError(string msg)
    {
        Cout.LogError(msg);
    }

    public void LogInfo(string msg)
    {
        Cout.println(msg);
    }
}
