using NetCommen;

namespace MainServer
{
    public class Server : BackgroundService
    {
        public readonly ILogger<Server> Logger;
        public readonly Network Network;
        public readonly ReciveHandler ReciveHandler;

        public Server(ILogger<Server> logger, Network network, ReciveHandler serverHandler)
        {
            Logger = logger;
            Network = network;
            ReciveHandler = serverHandler;

            Client.packetHandle = HandlePacket;
            NetworkCallbacks.createChallange = GenerateChallange;
            NetworkCallbacks.checkChalange = CheckChallange;
        }

        private int CheckChallange(int answer)
        {
            return (answer * 5) << 2;
        }

        private void HandlePacket(int clientId, Packet packet)
        {
            int packetID = packet.ReadInt();

            switch (packetID)
            {
                case NETWORK_COMMANDS.CS_ALIVE:
                    Network.GetClient(clientId).tcp.WriteHandshake();
                    return;
                case NETWORK_COMMANDS.CS_Handshake:
                    if (ReciveHandler.CS_Handshake(clientId, packet))
                    {
                        OnClientConnect(Network.GetClient(clientId));
                    }
                    return;
            }

            Ovr_HandlePacket(clientId, packet, packetID);
        }

        public int GenerateChallange()
        {
            return 5;
        }

        public virtual async Task ServerTick() => await Task.Delay(1);
        public virtual void Ovr_HandlePacket(int clientId, Packet packet, int packetID) { }
        public virtual void OnClientConnect(Client? c)
        {
            if (c == null)
                return;

            Packet packet = new Packet(NETWORK_COMMANDS.SC_SYNC);
            packet.WriteError(false);
            packet.Write(1);
            packet.Write(1001);
            packet.WriteLength();
            c.tcp.SendData(packet);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Network.Start();

            while (!stoppingToken.IsCancellationRequested)
            {
                await ServerTick();
            }
        }
    }
}
