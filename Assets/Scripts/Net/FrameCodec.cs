using System;
using System.Collections.Generic;
using System.IO;

namespace NRO.Net
{
    public static class FrameCodec
    {
        public static int MaxFrameBytes = 8 * 1024 * 1024; // 8MB

        public static RawPacket? Decode(Stream stream, StreamCipher cipher, bool keyReady, HashSet<sbyte> bigCmds)
        {
            byte[] headerBuf = new byte[1];
            if (!ReadExactly(stream, headerBuf, 1)) return null;
            
            sbyte cmd = (sbyte)headerBuf[0];
            if (keyReady)
            {
                cmd = cipher.Read(cmd);
            }

            int len = 0;
            if (bigCmds.Contains(cmd))
            {
                byte[] lenBuf = new byte[3];
                if (!ReadExactly(stream, lenBuf, 3)) return null;

                sbyte n1 = (sbyte)lenBuf[0];
                sbyte n2 = (sbyte)lenBuf[1];
                sbyte n3 = (sbyte)lenBuf[2];

                // Note: The original code always called readKey here, even if key Ready was false.
                // Our StreamCipher handles null key by returning the original byte.
                int num = cipher.Read(n1) + 128;
                int num2 = cipher.Read(n2) + 128;
                int num3 = cipher.Read(n3) + 128;
                len = (num3 * 256 + num2) * 256 + num;
            }
            else
            {
                byte[] lenBuf = new byte[2];
                if (!ReadExactly(stream, lenBuf, 2)) return null;

                sbyte b2 = (sbyte)lenBuf[0];
                sbyte b3 = (sbyte)lenBuf[1];

                if (keyReady)
                {
                    len = ((cipher.Read(b2) & 0xFF) << 8) | (cipher.Read(b3) & 0xFF);
                }
                else
                {
                    // Quirk kept intentionally:
                    len = (b2 & 0xFF00) | (b3 & 0xFF);
                }
            }

            if (len < 0 || len > MaxFrameBytes)
            {
                throw new InvalidDataException(string.Format("Invalid frame length: {0}", len));
            }

            sbyte[] payload = new sbyte[len];
            if (len > 0)
            {
                byte[] payloadBuf = new byte[len];
                if (!ReadExactly(stream, payloadBuf, len)) return null;

                for (int i = 0; i < len; i++)
                {
                    sbyte b = (sbyte)payloadBuf[i];
                    payload[i] = keyReady ? cipher.Read(b) : b;
                }
            }

            return new RawPacket(cmd, payload);
        }

        public static void Encode(RawPacket packet, StreamCipher cipher, bool keyReady, byte[] scratch, Stream stream, out int length)
        {
            int idx = 0;
            
            if (keyReady)
            {
                scratch[idx++] = (byte)cipher.Write(packet.Command);
            }
            else
            {
                scratch[idx++] = (byte)packet.Command;
            }

            int payloadLen = packet.Length;

            if (keyReady)
            {
                scratch[idx++] = (byte)cipher.Write((sbyte)(payloadLen >> 8));
                scratch[idx++] = (byte)cipher.Write((sbyte)(payloadLen & 0xFF));

                if (payloadLen > 0 && packet.Payload != null)
                {
                    for (int i = 0; i < payloadLen; i++)
                    {
                        scratch[idx++] = (byte)cipher.Write(packet.Payload[i]);
                    }
                }
            }
            else
            {
                // Little-endian ushort for length when key is not ready, as per original BinaryWriter quirk.
                scratch[idx++] = (byte)(payloadLen & 0xFF);
                scratch[idx++] = (byte)(payloadLen >> 8);

                // Note: The original code DID NOT WRITE PAYLOAD if !keyReady and data != null.
                // We preserve this behavior.
            }

            length = idx;
            stream.Write(scratch, 0, length);
            stream.Flush();
        }

        private static bool ReadExactly(Stream stream, byte[] buffer, int count)
        {
            int offset = 0;
            while (offset < count)
            {
                int read = stream.Read(buffer, offset, count - offset);
                if (read == 0) return false; // EOF
                offset += read;
            }
            return true;
        }
    }
}
