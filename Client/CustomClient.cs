using Algo;
using ClientCommon;
using NetCommen;
using NetCommen.Commands;
using TestCommon;

namespace Client
{
    public class CustomClient : ClientSystem
    {
        long lastPing = 0;

        public CustomClient() : base()
        {
            Encryption e = new Encryption();
            Decryption d = new Decryption();

            checkChalange = CheckChallange;
        }

        public override void OnDisconect(int id)
        {
            System.Environment.Exit(0);
        }

        public void Login()
        {
            string username = "User"; // lConsole.ReadLine();
            string password = "password"; // Console.ReadLine();

            Console.ReadLine();

            Packet pkt = new Packet(NETWORK_COMMANDS.CS_Action);
            pkt.EncryptFlag = true;
            pkt.Write(false);
            pkt.Write(1);
            pkt.Write(username);
            pkt.Write(password);
            pkt.WriteLength();
            Client.NetworkClient[1].SendData(pkt);
        }

        public override void OnConnected()
        {
            Login();

            while (true)
            {
                string? chat = Console.ReadLine();
                if (chat == null)
                    continue;
                if (chat.StartsWith("/"))
                {
                    NetworkCommand networkCommand = new NetworkCommand();
                    networkCommand.Arguments = chat.Split(' ');
                    switch (networkCommand.Arguments[0].ToLower())
                    {
                        case "/ping":
                            lastPing = DateTime.Now.Ticks;
                            networkCommand.CommandId = 1;
                            break;
                        case "/disconect":
                            networkCommand.CommandId = 2;
                            break;
                        case "/spawn":
                            networkCommand.CommandId = 5;
                            break;
                    }
                    Client.tcp.SendData(networkCommand);
                }
            }
        }

        private int CheckChallange(int answer)
        {
            return (answer * 5) << 2;
        }

        public override void ovr_HandleData(int clientId, Packet packet, int PacketId)
        {
            // Console.WriteLine(PacketId);
            switch (PacketId)
            {
                case NETWORK_COMMANDS.SC_Ping:
                    long recivedPing = packet.ReadLong();
                    Console.WriteLine(recivedPing - lastPing);
                    return;
                case NETWORK_COMMANDS.SC_SYNC:
                    packet.ReadBool();
                    int syncId = packet.ReadInt();
                    switch (syncId)
                    {
                        case 5:
                            // New player spawn
                            Player p = new Player();
                            p.UnPack(packet);
                            Console.WriteLine($"Player({p.ObjectId}) spawned in");
                            Console.WriteLine($"    Name: {p.Name}");
                            return;
                    }
                    return;
            }
        }
    }
}
