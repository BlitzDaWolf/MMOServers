using Algo;
using NetCommen.Interface;
using System.Collections.Generic;
using System.Linq;

namespace NetCommen.Object
{
    public class NetworkObjectList : List<IPackage>, IPackage
    {
        public NetworkObjectList() { }
        public NetworkObjectList(IEnumerable<IPackage> collection) : base(collection) { }
        public NetworkObjectList(int capacity) : base(capacity) { }

        public List<Packet> unpackedPackets = new List<Packet>();

        public Packet ClientPack()
        {
            Packet pkt = new Packet(NETWORK_COMMANDS.List);

            pkt.Write(Count);

            foreach (IPackage p in this)
            {
                var part = p.ServerPack().WriteLength();

                if (part.EncryptFlag)
                {
                    byte[] before = part.ReadBytes(5);
                    byte[] result = Algo.Encryption.Instance.Encrypt(part.ReadBytes(part.UnreadLength()));
                    part = new Packet(before);
                    part.Write(result);
                }

                pkt.Write(part.ToArray());
            }

            return pkt;
        }

        public Packet ServerPack()
        {
            Packet pkt = new Packet(NETWORK_COMMANDS.List);

            pkt.Write(Count);

            foreach (IPackage p in this)
            {
                var part = p.ServerPack().WriteLength();

                if (part.EncryptFlag)
                {
                    byte[] before = part.ReadBytes(5);
                    byte[] result = Encryption.Instance.Encrypt(part.ReadBytes(part.UnreadLength()));
                    part = new Packet(before);
                    part.Write(result);
                }

                pkt.Write(part.ToArray());
            }

            return pkt;
        }

        public void UnPack(Packet pkt)
        {
            int packets = pkt.ReadInt();
            for (int i = 0; i < packets; i++)
            {
                Packet _packet  = new Packet(pkt.ReadBytes(pkt.ReadInt(false) + 4));
                _packet.ReadInt();
                bool encrypted = _packet.ReadBool();

                if (encrypted)
                {
                    _packet = new Packet(_packet.ToArray());

                    List<byte> before = _packet.ReadBytes(5).ToList();
                    int toRead = _packet.UnreadLength();

                    byte[] mid = _packet.ReadBytes(toRead, false);
                    byte[] after = Decryption.Instance.Decrypt(mid);

                    before.AddRange(after);

                    _packet = new Packet(before.ToArray());
                    _packet.ReadBytes(5);
                }

                unpackedPackets.Add(_packet);
            }
        }
    }
}
