namespace NetCommen
{
    public class AdminCommands :NetworkCommand
    {
        public int UserId { get; set; }

        public override Packet ClientPack()
        {
            Packet pkt = base.ClientPack();
            pkt.Write(UserId);
            return pkt;
        }

        public override void UnPack(Packet pkt)
        {
            base.UnPack(pkt);
            UserId = pkt.ReadInt();
        }
    }
}
