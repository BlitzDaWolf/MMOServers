using DynamicServer;
using NetCommen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestServer
{
    public class EventConnection : ClientConnect
    {
        public EventConnection(ILogger<EventConnection> logger, IConfiguration config) : base(logger)
        {
            Ip = config.GetValue<string>("eventserver:ip");
            Port = config.GetValue<int>("eventserver:port");

            udp.ip = Ip;
            udp.port = Port;

            checkChalange = (int q) =>
            {
                return (q * 5) << 2;
            };
        }

        public override void OnPacket(Packet packet, int PacketId)
        {

        }
    }
}
