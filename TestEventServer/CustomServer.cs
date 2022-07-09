using EventCommon;
using EventServer;
using MainServer;
using NetCommen;

namespace TestEventServer
{
    public class CustomServer : Server
    {
        public ServerHandler ServerHandler { get; private set; }
        public List<Event> ActiveEvents { get; private set; } = new List<Event>();

        public CustomServer(ILogger<CustomServer> logger, Network network, ServerHandler handler, ReciveHandler reciveHandler) :
            base(logger, network, reciveHandler)
        {
            ServerHandler = handler;

            ActiveEvents.Add(new Event { EventDetails = 1000, EventId = 5 });
            ActiveEvents.Add(new Event { EventDetails = 1001, EventId = 5 });
        }

        public override async Task ServerTick()
        {
            int clients = Network.ConnectedClients;
            if (clients == 0)
                return;

        }

        public override void Ovr_HandlePacket(int clientId, Packet packet, int packetID)
        {
            Logger.LogInformation("Testing");
        }

        public override void OnClientConnect(Client c)
        {
            Logger.LogInformation("A new player joined the server");
            foreach (var item in ActiveEvents)
            {
                c.udp.SendData(item);
            }
        }
    }
}