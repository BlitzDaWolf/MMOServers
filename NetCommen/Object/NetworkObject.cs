using NetCommen;
using NetCommen.Interface;
using System.Reflection;

namespace NetCommen.Object
{
    public abstract class NetworkObject : IPackage
    {
        public int commandId { get; set; } = -1;
        public int ObjectId { get; set; } = -1;
        public bool SyncToAllClients { get; set; } = true;
        public bool LocalObject { get; set; } = false;

        public byte[] otherBytes { get; set; } = new byte[0];

        private Packet Write(Packet pkt)
        {
            pkt.Write(false);
            pkt.Write(commandId);

            pkt.Write(ObjectId);
            pkt.Write(SyncToAllClients);

            return pkt;
        }

        public virtual Packet ClientPack()
        {
            Packet pkt = new Packet(NETWORK_COMMANDS.CS_SYNC);
            return Write(pkt);
        }

        public virtual Packet ServerPack()
        {
            Packet pkt = new Packet(NETWORK_COMMANDS.SC_SYNC);
            return Write(pkt);
        }

        public virtual void UnPack(Packet pkt)
        {
            ObjectId = pkt.ReadInt();
            SyncToAllClients = pkt.ReadBool();
        }
    }
}
