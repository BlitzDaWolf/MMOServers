using ClientCommon;
using NetCommen;

namespace MainServer
{
    public class EventConnection : ClientSystem
    {
        public readonly ILogger<EventConnection> Logger;
        readonly int port;
        readonly string ip;

        public EventConnection(ILogger<EventConnection> logger, IConfiguration configuration) : base()
        {
            port = configuration.GetValue<int>("eventserver:port");
            ip = configuration.GetValue<string>("eventserver:ip");

            Logger = logger;
        }

        public void Connect()
        {
            Connect(ip, port);
        }

        public override void ovr_HandleData(int clientId, Packet packet, int PacketId)
        {

        }
    }
}
