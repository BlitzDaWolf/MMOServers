using NetCommen;
using NetCommen.Interface;

namespace EventCommon
{
    public class Event : IPackage
    {
        public int EventId { get; set; } = 1;
        public int EventDetails { get; set; } = -1;

        private Packet WritePacket(int command)
        {
            Packet packet = new Packet(command);

            packet.Write(EventId);
            packet.Write(EventDetails);

            return packet;
        }

        public virtual void LoadEvent() { }

        public Packet ClientPack()
        {
            return WritePacket(NETWORK_COMMANDS.SE_EventDetail);
        }

        public Packet ServerPack()
        {
            return WritePacket(NETWORK_COMMANDS.ES_EventDetail);
        }

        public void UnPack(Packet pkt)
        {
            EventId         = pkt.ReadInt();
            EventDetails    = pkt.ReadInt();
        }
    }
}