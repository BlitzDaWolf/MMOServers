using MainServer;
using NetCommen;

namespace TestEventServer
{
    public class ServerHandler
    {
        public ILogger<ServerHandler> Logger { get; private set; }
        public Network Network { get; private set; }

        public ServerHandler(ILogger<ServerHandler> logger, Network network)
        {
            Logger = logger;
            Network = network;

            ReciveHandler.CS_PlayerAction = CS_PlayerAction;
            ReciveHandler.CS_PlayerWarp = CS_PlayerWarp;
            ReciveHandler.CS_PlayerDie = CS_PlayerDie;
        }

        public void CS_PlayerAction(int id, Packet pkt, int packetId)
        {
            int actionId = pkt.ReadInt();
            if(actionId == 5)
            {

            }
            if(actionId == 15)
            {
                string username = pkt.ReadString();
                float health = pkt.ReadFloat();

                Logger.LogInformation($"User: {username} send health: {health}");
            }
        }

        public void CS_PlayerWarp (int id, Packet pkt)
        {

        }

        public void CS_PlayerDie(int id, Packet pkt)
        {

        }
    }
}
