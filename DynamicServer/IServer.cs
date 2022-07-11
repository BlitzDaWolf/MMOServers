using Microsoft.Extensions.Hosting;
using NetCommen;
using NetCommen.NetworkClient;

namespace DynamicServer
{
    public interface IServer
    {
        void HandlePacket(int clientId, Packet packet);
        void OnHandlePacket(int clientId, Packet packet, int packetID) { }
        void OnClientConnect(Client? c);
        Task ServerTick();
    }
}
