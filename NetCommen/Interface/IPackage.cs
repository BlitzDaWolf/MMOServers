namespace NetCommen.Interface
{
    public interface IPackage
    {
        Packet ServerPack();
        Packet ClientPack();

        void UnPack(Packet pkt);
    }
}
