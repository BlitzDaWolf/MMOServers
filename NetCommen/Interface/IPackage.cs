namespace NetCommen.Interface
{
    public interface IPackage
    {
        public Packet ServerPack();
        public Packet ClientPack();

        public void UnPack(Packet pkt);
    }
}
