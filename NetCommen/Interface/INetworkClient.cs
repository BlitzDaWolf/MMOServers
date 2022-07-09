namespace NetCommen.Interface
{
    public interface INetworkClient
    {
        void SendData(Packet packet);
        void SendData(IPackage packet);
        void Disconect();
        void Disconect(bool reason, string msg);
    }
}
