﻿using NetCommen;
using NetCommen.NetworkClient;
using NetCommen.Object;
using System;
using System.Net;
using System.Threading;

namespace ClientCommon
{
    public class ClientSystem
    {
        public readonly Client Client;
        public CheckChalange checkChalange = (int a) => 0;

        public ClientSystem()
        {
            Client = new Client(-1);
            Client.packetHandle = HandlePacket;
            Client.disconectHandler = OnDisconect;

            Client.Conncted = Connected;
        }

        public virtual void OnDisconect(int id) { }
        public virtual void OnConnected() { }

        /// <summary>
        /// Handles the default incoming packets
        /// </summary>
        /// <param name="clientId">Id of the client their socket</param>
        /// <param name="packet">Packet</param>
        public void HandlePacket(int clientId, Packet packet)
        {
            int packetId = packet.ReadInt();
            switch (packetId)
            {
                case NETWORK_COMMANDS.List:
                    NetworkObjectList list = new NetworkObjectList();
                    list.UnPack(packet);
                    foreach (var i in list.unpackedPackets)
                    {
                        HandlePacket(clientId, i);
                    }
                    return;
                case NETWORK_COMMANDS.SC_Handshake:
                    string NetworkVersion = packet.ReadString();
                    if (NetworkVersion != NETWORK_COMMANDS.NETWORK_VERSION)
                    {
                        Client.tcp.Disconect();
                    }
                    int challange = packet.ReadInt();
                    int response = checkChalange(challange);

                    Packet respPacket = new Packet(NETWORK_COMMANDS.CS_Handshake);
                    respPacket.Write(response);
                    respPacket.WriteLength();

                    Client.tcp.SendData(respPacket);
                    return;
                case NETWORK_COMMANDS.SC_ACK:
                    bool err = packet.ReadBool();
                    int id = packet.ReadInt();
                    IPEndPoint endPoint = (IPEndPoint)Client.tcp.socket.Client.LocalEndPoint;
                    if (endPoint == null)
                        return;

                    Client.id = id;
                    Client.tcp.id = id;
                    Client.udp.id = id;
                    Client.udp.Connect(endPoint.Port);

                    Console.WriteLine("Connection completed");
                    return;
            }
            ovr_HandleData(clientId, packet, packetId);
        }

        public virtual void ovr_HandleData(int clientId, Packet packet, int PacketId) { }

        /// <summary>
        /// If the TCP connection has been made send alive message
        /// </summary>
        public void Connected()
        {
            Packet pkt = new Packet(NETWORK_COMMANDS.CS_ALIVE);
            pkt.Write(0);
            pkt.WriteLength();
            Client.tcp.SendData(pkt);

            Thread t1 = new Thread(OnConnected);
            t1.Start();
        }

        /// <summary>
        /// Connect to the server
        /// </summary>
        /// <param name="ip">Server ip</param>
        /// <param name="port">Server port</param>
        public void Connect(string ip, int port)
        {
            try
            {
                Client.tcp.Connect(ip, port);
                Client.udp.ip = ip;
                Client.udp.port = port;
            }
            catch (Exception e)
            {

            }
        }
    }
}
