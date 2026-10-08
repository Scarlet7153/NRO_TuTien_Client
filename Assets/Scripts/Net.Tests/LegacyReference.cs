using System;
using System.IO;

namespace NRO.Net.Tests
{
    public class LegacyReference
    {
        public sbyte[] key;
        public sbyte curR;
        public sbyte curW;
        public bool getKeyComplete;

        public void SetKey(sbyte[] rawKey)
        {
            if (rawKey == null)
            {
                key = null;
                getKeyComplete = false;
                curR = 0;
                curW = 0;
                return;
            }
            key = new sbyte[rawKey.Length];
            Array.Copy(rawKey, key, rawKey.Length);
            for (int j = 0; j < key.Length - 1; j++)
            {
                key[j + 1] = (sbyte)(key[j + 1] ^ key[j]);
            }
            getKeyComplete = true;
            curR = 0;
            curW = 0;
        }

        public sbyte readKey(sbyte b)
        {
            if (key == null) return b;
            sbyte[] array = key;
            sbyte num = curR;
            curR = (sbyte)(num + 1);
            sbyte result = (sbyte)((array[num] & 0xFF) ^ (b & 0xFF));
            if (curR >= key.Length)
            {
                curR = (sbyte)(curR % (sbyte)key.Length);
            }
            return result;
        }

        public sbyte writeKey(sbyte b)
        {
            if (key == null) return b;
            sbyte[] array = key;
            sbyte num = curW;
            curW = (sbyte)(num + 1);
            sbyte result = (sbyte)((array[num] & 0xFF) ^ (b & 0xFF));
            if (curW >= key.Length)
            {
                curW = (sbyte)(curW % (sbyte)key.Length);
            }
            return result;
        }

        public RawPacket LegacyReadMessage(BinaryReader dis, bool[] bigSet)
        {
            sbyte b = dis.ReadSByte();
            if (getKeyComplete)
            {
                b = readKey(b);
            }
            // b + 128 maps to 0..255. So bigSet index must be positive. We pass bigSet as bool[256].
            if (bigSet[(b + 256) % 256])
            {
                int num = readKey(dis.ReadSByte()) + 128;
                int num2 = readKey(dis.ReadSByte()) + 128;
                int num3 = readKey(dis.ReadSByte()) + 128;
                int num4 = (num3 * 256 + num2) * 256 + num;
                sbyte[] array = new sbyte[num4];
                byte[] src = dis.ReadBytes(num4);
                
                // simulate ArrayCast.cast
                for(int i=0; i<num4; i++) array[i] = (sbyte)src[i];
                
                if (getKeyComplete)
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        array[i] = readKey(array[i]);
                    }
                }
                return new RawPacket(b, array);
            }
            
            int len;
            if (getKeyComplete)
            {
                sbyte b2 = dis.ReadSByte();
                sbyte b3 = dis.ReadSByte();
                len = ((readKey(b2) & 0xFF) << 8) | (readKey(b3) & 0xFF);
            }
            else
            {
                sbyte b4 = dis.ReadSByte();
                sbyte b5 = dis.ReadSByte();
                len = (b4 & 0xFF00) | (b5 & 0xFF);
            }
            sbyte[] array2 = new sbyte[len];
            if (len > 0)
            {
                byte[] src2 = dis.ReadBytes(len);
                for(int i=0; i<len; i++) array2[i] = (sbyte)src2[i];
                if (getKeyComplete)
                {
                    for (int i = 0; i < array2.Length; i++)
                    {
                        array2[i] = readKey(array2[i]);
                    }
                }
            }
            return new RawPacket(b, array2);
        }

        public void LegacyWriteMessage(BinaryWriter dos, RawPacket m)
        {
            sbyte[] data = m.Payload;
            if (getKeyComplete)
            {
                sbyte value = writeKey(m.Command);
                dos.Write(value);
            }
            else
            {
                dos.Write(m.Command);
            }
            
            if (data != null)
            {
                int num = data.Length;
                if (getKeyComplete)
                {
                    int num2 = writeKey((sbyte)(num >> 8));
                    dos.Write((sbyte)num2);
                    int num3 = writeKey((sbyte)(num & 0xFF));
                    dos.Write((sbyte)num3);
                }
                else
                {
                    dos.Write((ushort)num);
                }
                
                if (getKeyComplete)
                {
                    for (int i = 0; i < data.Length; i++)
                    {
                        sbyte value2 = writeKey(data[i]);
                        dos.Write(value2);
                    }
                }
            }
            else
            {
                if (getKeyComplete)
                {
                    int num4 = 0;
                    int num5 = writeKey((sbyte)(num4 >> 8));
                    dos.Write((sbyte)num5);
                    int num6 = writeKey((sbyte)(num4 & 0xFF));
                    dos.Write((sbyte)num6);
                }
                else
                {
                    dos.Write((ushort)0);
                }
            }
            dos.Flush();
        }
    }
}
