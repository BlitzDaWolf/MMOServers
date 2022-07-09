using EventCommon;
using MainServer;
using NetCommen;
using TestCommon;

namespace TestEventServer
{
    public class CustomServer : Server
    {
        public ServerHandler ServerHandler { get; private set; }
        public List<NPC> npcs { get; private set; } = new List<NPC>();

        public CustomServer(ILogger<CustomServer> logger, Network network, ServerHandler handler, ReciveHandler reciveHandler, EventConnection eventConnection) :
            base(logger, network, reciveHandler, eventConnection)
        {
            ServerHandler = handler;

            npcs.Add(new NPC { ObjectId = 1000, commandId= 2, Level = 30, Name = "Blacksmith" });
            npcs.Add(new NPC { ObjectId = 1001, commandId = 2, Level = 10, Name = "Gaurd" });
            npcs.Add(new NPC { ObjectId = 1001, commandId = 2, Level = 60, Name = "Theif" });
        }

        public override async Task ServerTick()
        {
            foreach (var item in npcs)
            {
                // Network.SendToAllClientsUdp(item.Move());
            }
            await Task.Delay(1);
        }

        public override void Ovr_HandlePacket(int clientId, Packet packet, int packetID)
        {
            // Logger.LogInformation($"Packet({packetID})");
            switch (packetID)
            {
                case NETWORK_COMMANDS.ES_EventDetail:
                    Event e = new Event();
                    e.UnPack(packet);
                    Logger.LogInformation($"Event({e.EventId})");
                    return;
            }
        }

        public override void OnClientConnect(Client c)
        {
            Logger.LogInformation("A new player joined the server");
            foreach (var item in npcs)
            {
                c.tcp.SendData(item);
            }
        }
    }
}