using MainServer;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetCommen;
using NetCommen.NetworkClient;
using NetCommen.Object;
using System.Diagnostics;

namespace EventServer
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

            Network.packetHandle = HandlePacket;
            NetworkCallbacks.createChallange = GenerateChallange;
            NetworkCallbacks.checkChalange = CheckChallange;
            NetworkCallbacks.OnClientConnected = OnClientConnect;
        }

        private int CheckChallange(int answer)
        {
            return (answer * 5) << 2;
        }

        /// <summary>
        /// Handles the default incoming packets
        /// </summary>
        /// <param name="clientId">Id of the client their socket</param>
        /// <param name="packet">Packet</param>
        private void HandlePacket(int clientId, Packet packet)
        {
            int packetID = packet.ReadInt();

            switch (packetID)
            {
                case NETWORK_COMMANDS.List:
                    NetworkObjectList list = new NetworkObjectList();
                    list.UnPack(packet);
                    foreach (var i in list.unpackedPackets)
                    {
                        HandlePacket(clientId, i);
                    }
                    return;
                case NETWORK_COMMANDS.CS_ALIVE:
                    Network.GetClient(clientId).tcp.WriteHandshake();
                    return;
                case NETWORK_COMMANDS.CS_Handshake:
                    if (ReciveHandler.CS_Handshake(clientId, packet))
                    {
                        // OnClientConnect(Network.GetClient(clientId));
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
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Network.Start();

            double t = 1d / 40d;

            int ticks = 0;
            long nextLastTick = (int)(10000000 * t);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (!stoppingToken.IsCancellationRequested)
            {                
                await ServerTick();

                while (stopwatch.Elapsed.Ticks < nextLastTick)
                    await Task.Delay(1);

                nextLastTick += (int)(10000000 * t);
                ticks++;
            }
        }
    }
}
