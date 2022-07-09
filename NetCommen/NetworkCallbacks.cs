using System.Net;

namespace NetCommen
{
    public delegate void OnConnected();

    public delegate void PacketHandler(int clientId, Packet packet);
    public delegate void DisconectHandler(int clientId);

    public delegate void OnClientConnected(Client c);
    public delegate void SendData(IPEndPoint endPoint, Packet _packet);

    public delegate int CreateChallange();
    public delegate int CheckChalange(int answer);

    public static class NetworkCallbacks
    {
        public static SendData sendData = (IPEndPoint endPoint, Packet _packet) => { };

        public static CreateChallange createChallange = () => 0;
        public static CheckChalange checkChalange = (int a) => 0;

        public static OnClientConnected OnClientConnected = (Client c) => { };
        public static OnConnected Conncted = () => { };
    }
}
