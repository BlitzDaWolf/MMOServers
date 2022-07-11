using DynamicServer;
using EventCommon;
using MainServer;
using NetCommen;
using NetCommen.NetworkClient;
using NetCommen.Object;
using ServerCommon;
using TestCommon;

namespace TestServer
{
    public class CustomServer : Server
    {
        public readonly EventConnection EventConnection;

        public List<NPC> npcs { get; private set; } = new List<NPC>();
        public Dictionary<int, Player> players { get; set; } = new Dictionary<int, Player>();

        public static CustomServer Instance { get; internal set; }

        public CustomServer(
            ILogger<CustomServer> logger,
            Network network,
            ReciveHandler reciveHandler,
            CCommandHandler commandHandler,
            EventConnection eventConnection) :
            base(logger, network, reciveHandler)
        {
            this.EventConnection = eventConnection;

            Instance = this;

            CommandHandler.Commands.Add(1, commandHandler.HandlePing);
            CommandHandler.Commands.Add(2, commandHandler.HandleDisconect);
            CommandHandler.Commands.Add(5, commandHandler.HandleSpawn);

            npcs.Add(new NPC { commandId = 2, Level = 1, ObjectId = 1001 });
            npcs.Add(new NPC { commandId = 2, Level = 1, ObjectId = 1002 });
            npcs.Add(new NPC { commandId = 2, Level = 1, ObjectId = 1003 });
        }

        public override async Task ServerTick()
        {
            NetworkObjectList list = new NetworkObjectList();

            Network.SendToAllClients(list, 1);
            await Task.Delay(1);
        }

        public override void OnHandlePacket(int clientId, Packet packet, int packetID)
        {
            // Logger.LogInformation($"Packet({packetID})");
            switch (packetID)
            {
                case NETWORK_COMMANDS.ES_EventDetail:
                    Event e = new Event();
                    e.UnPack(packet);
                    Logger.LogInformation($"Event({e.EventId})");
                    return;
                case NETWORK_COMMANDS.CS_Action:
                    bool error = packet.ReadBool();
                    int commandID = packet.ReadInt();
                    switch (commandID)
                    {
                        case 1:
                            if (players.ContainsKey(clientId))
                            {
                                if (players[clientId].Loggedin == true)
                                    return;
                                string Username = packet.ReadString();
                                string Password = packet.ReadString();
                                // Validate user thruw private webserver
                                players[clientId].Loggedin = true;
                                players[clientId].Admin = true;
                                players[clientId].Name = Username;

                                NetworkObjectList list = new NetworkObjectList();

                                foreach (var player in players.Values.Where(x => x.Loggedin))
                                {
                                    list.Add(player);
                                }
                                Network.GetClient(clientId)?.NetworkClient[1].SendData(list);
                                Network.SendToAllClients(players[clientId], 1, clientId);
                            }
                            return;
                    }
                    return;
            }
        }

        public override void OnClientConnect(Client c)
        {
            if (c == null)
                return;
            Logger.LogInformation("A new player joined the server");
            foreach (var item in npcs)
            {
                c.udp.SendData(item);
            }

            Player p = new Player();
            p.Level = 1;
            p.Exp = 0;
            p.Name = "";
            p.ObjectId = c.id;
            p.commandId = 5;
            if (players.ContainsKey(p.ObjectId))
                players[p.ObjectId] = p;
            else
                players.Add(p.ObjectId, p);
        }

        public override void OnClientDisconect(int clientId)
        {
            if (players.ContainsKey(clientId))
            {
                players.Remove(clientId);
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            EventConnection.Connect();
            return base.ExecuteAsync(stoppingToken);
        }
    }
}