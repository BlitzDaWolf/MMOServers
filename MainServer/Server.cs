﻿using NetCommen;
using ServerCommon;
using System.Diagnostics;

namespace MainServer
{
    public class Server : BackgroundService
    {
        public readonly ILogger<Server> Logger;
        public readonly Network Network;
        public readonly ReciveHandler ReciveHandler;
        public readonly EventConnection EventServer;

        public Server(ILogger<Server> logger, Network network, ReciveHandler serverHandler, EventConnection eventServer)
        {
            Logger = logger;
            Network = network;
            ReciveHandler = serverHandler;
            EventServer = eventServer;

            Network.packetHandle = HandlePacket;
            Network.disconectHandler = OnClientDisconect;

            NetworkCallbacks.createChallange = GenerateChallange;
            NetworkCallbacks.checkChalange = CheckChallange;

            Network.Connected = OnClientConnect;
        }

        /// <summary>
        /// Challange verification
        /// </summary>
        private int CheckChallange(int answer)
        {
            return (answer * 5) << 2;
        }

        /// <summary>
        /// Generates the challange for the handshake
        /// </summary>
        /// <returns>Challange</returns>
        public int GenerateChallange()
        {
            return 5;
        }

        /// <summary>
        /// Handles the default incoming packets
        /// </summary>
        /// <param name="clientId">Id of the client their socket</param>
        /// <param name="packet">Packet</param>
        public void HandlePacket(int clientId, Packet packet)
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
                case NETWORK_COMMANDS.SC_Handshake:
                case NETWORK_COMMANDS.SC_ACK:
                    packet.Reset(false);
                    EventServer.HandlePacket(clientId, packet);
                    return;
                case NETWORK_COMMANDS.CS_Command:
                    bool isAdminCmd = packet.ReadBool();
                    if (isAdminCmd)
                    {
                        AdminCommands acminCommand = new AdminCommands();
                        acminCommand.UnPack(packet);
                        CommandHandler.ExecuteAdminCommand(clientId, acminCommand);
                    }
                    else
                    {
                        NetworkCommand command = new NetworkCommand();
                        command.UnPack(packet);
                        CommandHandler.ExecuteCommand(clientId, command);
                    }
                    return;
            }

            Ovr_HandlePacket(clientId, packet, packetID);
        }

        /// <summary>
        /// Executes evry update tick
        /// </summary>
        public virtual async Task ServerTick() => await Task.Delay(1);
        public virtual void Ovr_HandlePacket(int clientId, Packet packet, int packetID) { }
        public virtual void OnClientConnect(Client? c)
        {
            if (c == null)
                return;
        }
        public virtual void OnClientDisconect(int clientId) { }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Network.Start();
            EventServer.Connect();

            double t = 1d / 40d;

            int ticks = 0;
            long nextLastTick = (int)(10000000 * t);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (!stoppingToken.IsCancellationRequested)
            {
                /*if (ticks % 40 == 0)
                {
                    if (ticks != 0)
                    {
                        long tps = ticks / (stopwatch.Elapsed.Ticks / 10000000);
                        Logger.LogInformation($"{stopwatch.Elapsed}/{tps}");
                    }
                }*/

                await ServerTick();


                while (stopwatch.Elapsed.Ticks < nextLastTick)
                    await Task.Delay(1);

                nextLastTick += (int)(10000000 * t);
                ticks++;
            }
        }
    }
}
