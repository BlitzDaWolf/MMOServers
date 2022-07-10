using NetCommen.Interface;

namespace NetCommen.Object
{
    public class NetworkObjectList : List<IPackage>, IPackage
    {
        public List<Packet> unpackedPackets = new List<Packet>();

        public Packet ClientPack()
        {
            Packet pkt = new Packet(NETWORK_COMMANDS.List);

            pkt.Write(Count);

            foreach (IPackage p in this)
            {
                pkt.Write(p.ClientPack().ToArray());
            }

            return pkt;
        }

        public Packet ServerPack()
        {
            Packet pkt = new Packet(NETWORK_COMMANDS.List);

            pkt.Write(Count);

            foreach (IPackage p in this)
            {
                pkt.Write(p.ServerPack().WriteLength().ToArray());
            }

            return pkt;
        }

        public void UnPack(Packet pkt)
        {
            int packets = pkt.ReadInt();
            for (int i = 0; i < packets; i++)
            {
                Packet p = new Packet(pkt.ReadBytes(pkt.ReadInt()));
                unpackedPackets.Add(p);
            }
        }
    }
}
