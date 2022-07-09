using MainServer;
using NetCommen;

namespace TestEventServer
{
    public class ServerHandler
    {
        public ILogger<ServerHandler> Logger { get; private set; }
        public Network Network { get; private set; }

        public ServerHandler(ILogger<ServerHandler> logger, Network network)
        {
            Logger = logger;
            Network = network;
        }
    }
}
