using NetCommen;
using NetCommen.Object;

namespace TestCommon
{
    public class Player : NetworkObject
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public float Exp { get; set; }
        public bool Loggedin { get; set; }
        public bool Admin { get; set; }

        public override Packet ServerPack()
        {
            return base.ServerPack()
                .Write(Name)
                .Write(Level)
                .Write(Exp);
        }

        public override Packet ClientPack()
        {
            return new Packet();
        }

        public override void UnPack(Packet pkt)
        {
            base.UnPack(pkt);
            Name = pkt.ReadString();
            Level = pkt.ReadInt();
            Exp = pkt.ReadFloat();
        }
    }
}
