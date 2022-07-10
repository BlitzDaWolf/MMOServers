using System;
using System.Collections.Generic;
using System.Text;

namespace Algo
{
    public static class Decryption
    {
        public static byte[] Decrypt(byte[] value, byte[] Key)
        {
            List<byte> result = new List<byte>();

            for (int i = 0; i < value.Length; i++)
            {
                int r = (Key[i % 4] ^ value[i]);
                result.Add((byte)r);
            }

            return result.ToArray();
        }
    }
}
