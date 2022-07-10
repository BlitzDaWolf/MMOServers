using System;
using System.Collections.Generic;
using System.Text;

namespace Algo
{
    public static class Encription
    {
        public static byte[] Encrypt(byte[] value, byte[] Key)
        {
            List<byte> result = new List<byte>();

            for (int i = 0; i < value.Length; i++)
            {
                int r = (value[i] ^ Key[i % 4]);
                result.Add((byte)r);
            }

            return result.ToArray();
        }
    }
}
