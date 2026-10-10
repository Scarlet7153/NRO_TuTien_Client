using System;

namespace NRO.Net
{
    public struct RawPacket
    {
        public sbyte Command;
        public sbyte[] Payload;
        public int Length;
        
        public RawPacket(sbyte command, sbyte[] payload)
        {
            Command = command;
            Payload = payload;
            Length = payload != null ? payload.Length : 0;
        }
    }
}
