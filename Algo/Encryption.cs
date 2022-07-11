using System.Collections.Generic;

namespace Algo
{
    public class Encryption
    {
        private static Encryption _instance;
        public static Encryption Instance
        {
            get
            {
                if( _instance == null )
                    _instance = new Encryption();
                return _instance;
            }
            set { _instance = value; }
        }

        public byte[] Key = new byte[4] { 0x50, 0x5f, 0x9a, 0x84 };

        public virtual byte[] Encrypt(byte[] value)
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
