using MainServer;
using NetCommen;
using NetCommen.NetworkClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace TestServer
{
    public class CCommandHandler
    {
        public ILogger<CCommandHandler> Logger { get; }
        public Network Network { get; }
        public EventConnection Connection { get; }
        public CustomServer Server { get => CustomServer.Instance; }

        public CCommandHandler(
            ILogger<CCommandHandler> logger,
            Network network,
            EventConnection connection)
        {
            Logger = logger;
            Network = network;
            Connection = connection;
        }

        public void HandlePing(int clientId, string[] arguments)
        {
            Client? c = Network.GetClient(clientId);
            if (c == null)
                return;
            Packet pingPkt = new Packet(NETWORK_COMMANDS.SC_Ping);
            pingPkt.Write(DateTime.Now.Ticks);
            pingPkt.WriteLength();
            c.tcp.SendData(pingPkt);
        }

        public void HandleSave(int clientId, string[] arguments)
        {

        }

        public void HandleDisconect(int clientId, string[] arguments)
        {
            if (arguments.Length == 0)
                return;
            try
            {
                int disconectUser = 0;
                int.TryParse(arguments[0], out disconectUser);
                Client? selected = Network.GetClient(disconectUser);

                if (selected != null)
                    selected.Disconect();
            }catch (Exception ex)
            {

            }
        }

        public void HandleSpawn(int clientId, string[] arguments)
        {
            NPC newNpc = new NPC();
            newNpc.Name = arguments[0];
            newNpc.Level = int.Parse(arguments[1]);
            newNpc.ObjectId = int.Parse(arguments[2]);

            Server.npcs.Add(newNpc);
            Network.SendToAllClients(newNpc);
        }
    }
}
