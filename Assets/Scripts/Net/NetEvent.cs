using System;

namespace NRO.Net
{
    public enum NetEventType
    {
        Message,
        Connected,
        ConnectFailed,
        Disconnected,
        Handshake
    }

    public struct NetEvent
    {
        public NetEventType Type;
        public RawPacket Packet;
        public int Generation;
    }

    public interface INetLog
    {
        void LogError(string msg);
        void LogInfo(string msg);
    }
}
