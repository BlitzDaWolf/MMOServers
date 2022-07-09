namespace NetCommen.Interface
{
    public interface INetworkClient
    {
        /// <summary>
        /// Sends a packet
        /// </summary>
        /// <param name="packet">packet to send</param>
        void SendData(Packet packet);
        /// <summary>
        /// Sends a package
        /// </summary>
        /// <param name="packet">package to send</param>
        void SendData(IPackage packet);
        void Disconect();
        void Disconect(bool reason, string msg);
    }
}
