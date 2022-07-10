using Microsoft.Extensions.Logging;
using NetCommen;
using NetCommen.NetworkClient;
using NetCommen.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DynamicServer
{
    public abstract class ClientConnect : Client
    {
        public readonly ILogger<ClientConnect> Logger;


        protected CheckChalange checkChalange = (int a) => 0;
        public string Ip { get; set; } = "127.0.0.1";
        public int Port { get; set; }

        protected ClientConnect(ILogger<ClientConnect> logger) : base(0)
        {
            packetHandle = this.handlePacket;
            disconectHandler = this.OnDisconect;
            this.Logger = logger;

            Conncted = Connected;
        }

        private void Connected()
        {
            Packet packet = new Packet(403);
            packet.Write(0);
            packet.WriteLength();
            tcp.SendData(packet);
            Thread thread = new Thread(OnConnected);
            thread.Start();
        }

        public virtual void OnConnected() { }

        private void handlePacket(int clientId, Packet packet)
        {
            int packetId = packet.ReadInt();
            switch (packetId)
            {
                case NETWORK_COMMANDS.List:
                    {
                        NetworkObjectList list = new NetworkObjectList();
                        list.UnPack(packet);
                        foreach (var i in list.unpackedPackets)
                        {
                            handlePacket(clientId, i);
                        }
                        return;
                    }
                case NETWORK_COMMANDS.SC_Handshake:
                    {
                        string NetworkVersion = packet.ReadString();
                        if (NetworkVersion != NETWORK_COMMANDS.NETWORK_VERSION)
                        {
                            tcp.Disconect();
                        }
                        int challange = packet.ReadInt();
                        int response = checkChalange(challange);

                        Packet respPacket = new Packet(NETWORK_COMMANDS.CS_Handshake);
                        respPacket.Write(response);
                        respPacket.WriteLength();

                        tcp.SendData(respPacket);
                        return;
                    }
                case NETWORK_COMMANDS.SC_ACK:
                    {
                        bool err = packet.ReadBool();
                        int id = packet.ReadInt();
                        IPEndPoint endPoint = (IPEndPoint)tcp.socket.Client.LocalEndPoint;
                        if (endPoint == null)
                            return;

                        this.id = id;
                        tcp.id = id;
                        udp.id = id;
                        udp.Connect(endPoint.Port);
                        return;
                    }
            }
            OnPacket(packet, packetId);
        }

        public virtual void OnPacket(Packet packet, int PacketId) { }

        protected virtual void OnDisconect(int clientId)
        {
            Logger.LogInformation("Event server has closed");
            Environment.Exit(0);
        }

        public void Connect()
        {
            tcp.Connect(Ip, Port);
        }
    }
}
