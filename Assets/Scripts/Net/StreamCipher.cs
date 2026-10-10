using System;

namespace NRO.Net
{
    public class StreamCipher
    {
        private sbyte[] _key;
        private int _curR;
        private int _curW;

        public void SetKey(sbyte[] rawKey)
        {
            if (rawKey == null || rawKey.Length == 0)
            {
                _key = null;
                _curR = 0;
                _curW = 0;
                return;
            }

            _key = new sbyte[rawKey.Length];
            Array.Copy(rawKey, _key, rawKey.Length);

            for (int j = 0; j < _key.Length - 1; j++)
            {
                _key[j + 1] = (sbyte)(_key[j + 1] ^ _key[j]);
            }
            
            _curR = 0;
            _curW = 0;
        }

        public void Reset()
        {
            _key = null;
            _curR = 0;
            _curW = 0;
        }

        public sbyte Read(sbyte b)
        {
            if (_key == null) return b;

            int num = _curR;
            _curR++;
            sbyte result = (sbyte)((_key[num] & 0xFF) ^ (b & 0xFF));
            
            if (_curR >= _key.Length)
            {
                _curR %= _key.Length;
            }
            
            return result;
        }

        public sbyte Write(sbyte b)
        {
            if (_key == null) return b;

            int num = _curW;
            _curW++;
            sbyte result = (sbyte)((_key[num] & 0xFF) ^ (b & 0xFF));
            
            if (_curW >= _key.Length)
            {
                _curW %= _key.Length;
            }
            
            return result;
        }
    }
}
