using Microsoft.Extensions.Logging;
using NetCommen;
using NetCommen.NetworkClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainServer
{
    public delegate void CS_PlayerAction(int id, Packet pkt, int packetId);
    public delegate void CS_PlayerWarp(int id, Packet pkt);
    public delegate void CS_PlayerDie(int id, Packet pkt);

    /// <summary>
    /// Basic packet handeling
    /// </summary>
    public class ReciveHandler
    {
        public static CS_PlayerAction CS_PlayerAction = (int id, Packet pkt, int packetId) => { };
        public static CS_PlayerWarp CS_PlayerWarp = (int id, Packet pkt) => { };
        public static CS_PlayerDie CS_PlayerDie = (int id, Packet pkt) => { };

        public readonly Network Network;
        public readonly ILogger<ReciveHandler> Logger;

        public ReciveHandler(ILogger<ReciveHandler> logger, Network network)
        {
            this.Network = network;
            this.Logger = logger;
        }

        /// <summary>
        /// Check the client handshake
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pkt"></param>
        /// <returns>If the client has passed the challange</returns>
        public bool CS_Handshake(int id, Packet pkt)
        {
            Client? c = Network.GetClient(id);
            if (c == null)
                return false;
            int challange = c.tcp.challange;
            int neededResponse = NetworkCallbacks.checkChalange(challange);
            int response = pkt.ReadInt();
            if(neededResponse == response)
            {
                Logger.LogInformation($"Client({id}) succseded the handshake");

                Packet p = new Packet(NETWORK_COMMANDS.SC_ACK);
                p.WriteError(false);
                p.Write(id);
                p.WriteLength();
                c.tcp.SendData(p);
                return true;
            }
            else
            {
                Logger.LogInformation($"N({neededResponse})");
                Logger.LogInformation($"R({response})");
                Logger.LogInformation($"Client({id}) failed the handshake");
                c.tcp.Disconect(false, "");
                return false;
            }
        }
    }
}
