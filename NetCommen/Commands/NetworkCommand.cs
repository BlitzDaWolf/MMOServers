using NetCommen.Interface;
using System.Linq;

namespace NetCommen.Commands
{
    public class NetworkCommand : IPackage
    {
        public bool AdminCommand { get; set; } = false;
        public int CommandId { get; set; }
        public string[] Arguments { get; set; }

        public virtual Packet ClientPack()
        {
            Packet pkt = new Packet(NETWORK_COMMANDS.CS_Command);

            pkt.Write(AdminCommand);
            pkt.Write(CommandId);

            string args = "";
            foreach(string arg in Arguments)
            {
                if (arg.StartsWith("/"))
                    continue;
                args += $"{arg} ";
            }

            pkt.Write(args);

            return pkt;
        }

        public virtual Packet ServerPack()
        {
            return new Packet();
        }

        public virtual void UnPack(Packet pkt)
        {
            CommandId = pkt.ReadInt();
            string t = pkt.ReadString();
            Arguments = t.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        }
    }
}
