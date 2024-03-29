﻿using System.Collections.Generic;

namespace Algo
{
    public class Decryption
    {
        private static Decryption _instance;
        public static Decryption Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Decryption();
                return _instance;
            }
            set { _instance = value; }
        }

        public byte[] Key = new byte[4] { 0x50, 0x5f, 0x9a, 0x84 };

        public virtual byte[] Decrypt(byte[] value)
        {
            List<byte> result = new List<byte>();

            for (int i = 0; i < value.Length; i++)
            {
                int r = (Key[i % 4] ^ value[i]);
                result.Add((byte)r);
            }

            return result.ToArray();
        }

        public virtual bool IsEncrypted(byte[] value)
        {
            return false;
        }
    }
}
