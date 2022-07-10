using Microsoft.Extensions.Hosting;
using NetCommen;
using NetCommen.NetworkClient;

namespace DynamicServer
{
    public interface IServer : IHostedService
    {
        void HandlePacket(int clientId, Packet packet);
        void Ovr_HandlePacket(int clientId, Packet packet, int packetID) { }
        void OnClientConnect(Client? c);
        Task ServerTick();
    }
}
