using System;
using System.IO;
using System.Collections.Generic;
using NUnit.Framework;
using NRO.Net;

namespace NRO.Net.Tests
{
    [TestFixture]
    public class CodecGoldenTest
    {
        private bool[] GetBigSetFlags()
        {
            sbyte[] cmds = new sbyte[] { -32, -66, 11, -67, -74, -87, 66 };
            bool[] set = new bool[256];
            foreach (sbyte b in cmds) set[(b + 256) % 256] = true;
            return set;
        }

        private HashSet<sbyte> GetBigSet()
        {
            return new HashSet<sbyte> { -32, -66, 11, -67, -74, -87, 66 };
        }

        [Test]
        public void Test_EncodeDecode_MatchesLegacy_WithRandomData()
        {
            Random rnd = new Random(12345);
            bool[] bigSetFlags = GetBigSetFlags();
            HashSet<sbyte> bigSet = GetBigSet();

            for (int i = 0; i < 10000; i++)
            {
                // Random key lengths: 1, 2, 127
                int[] keyLengths = new int[] { 1, 2, 127 };
                int keyLen = keyLengths[rnd.Next(keyLengths.Length)];
                sbyte[] key = new sbyte[keyLen];
                for (int k = 0; k < key.Length; k++) key[k] = (sbyte)rnd.Next(-128, 128);

                bool keyReady = rnd.Next(2) == 0;
                
                // Random payload length
                int payloadLen = rnd.Next(0, 1000); // 0 to 1000 bytes
                
                // Random command
                sbyte cmd = (sbyte)rnd.Next(-128, 128);

                sbyte[] payload = new sbyte[payloadLen];
                for(int p = 0; p < payloadLen; p++) payload[p] = (sbyte)rnd.Next(-128, 128);

                RawPacket packet = new RawPacket(cmd, payload);

                // --- LEGACY ENCODE ---
                LegacyReference legacy = new LegacyReference();
                if (keyReady) legacy.SetKey(key);
                else legacy.SetKey(null);

                MemoryStream legacyMs = new MemoryStream();
                BinaryWriter legacyWriter = new BinaryWriter(legacyMs);
                legacy.LegacyWriteMessage(legacyWriter, packet);
                byte[] legacyEncodedBytes = legacyMs.ToArray();

                // --- NEW ENCODE ---
                StreamCipher cipher = new StreamCipher();
                if (keyReady) cipher.SetKey(key);
                else cipher.Reset();

                MemoryStream newMs = new MemoryStream();
                byte[] scratch = new byte[8 * 1024 * 1024];
                int newLength = 0;
                FrameCodec.Encode(packet, cipher, keyReady, scratch, newMs, out newLength);
                byte[] newEncodedBytes = newMs.ToArray();

                // Assert encoded bytes match exactly
                Assert.AreEqual(legacyEncodedBytes.Length, newEncodedBytes.Length, $"Length mismatch at iteration {i}");
                for (int b = 0; b < legacyEncodedBytes.Length; b++)
                {
                    Assert.AreEqual(legacyEncodedBytes[b], newEncodedBytes[b], $"Byte mismatch at index {b}, iteration {i}");
                }

                // --- LEGACY DECODE ---
                if (keyReady) legacy.SetKey(key); // Reset cipher state
                else legacy.SetKey(null);
                
                legacyMs.Position = 0;
                BinaryReader legacyReader = new BinaryReader(legacyMs);
                RawPacket legacyDecoded = legacy.LegacyReadMessage(legacyReader, bigSetFlags);

                // --- NEW DECODE ---
                if (keyReady) cipher.SetKey(key);
                else cipher.Reset();

                newMs.Position = 0;
                RawPacket? newDecoded = FrameCodec.Decode(newMs, cipher, keyReady, bigSet);

                // Assert decoded packets match
                Assert.IsNotNull(newDecoded);
                Assert.AreEqual(legacyDecoded.Command, newDecoded.Value.Command);
                
                if (legacyDecoded.Payload == null) 
                {
                    Assert.IsTrue(newDecoded.Value.Payload == null || newDecoded.Value.Payload.Length == 0);
                }
                else
                {
                    Assert.AreEqual(legacyDecoded.Payload.Length, newDecoded.Value.Payload.Length);
                    for (int p = 0; p < legacyDecoded.Payload.Length; p++)
                    {
                        Assert.AreEqual(legacyDecoded.Payload[p], newDecoded.Value.Payload[p], $"Payload mismatch at index {p}, iteration {i}");
                    }
                }
            }
        }
    }
}
