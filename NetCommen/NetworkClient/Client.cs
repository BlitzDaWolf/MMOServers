using NetCommen.Interface;
using System.Net;
using System.Net.Sockets;

namespace NetCommen.NetworkClient
{
    public class Client
    {
        public const int dataBufferSize = 4096;

        public int id;
        public TCP tcp;
        public UDP udp;

        public bool IsConnected
        {
            get
            {
                return tcp.connected;
            }
        }

        public INetworkClient[] NetworkClient
        {
            get
            {
                return new INetworkClient[2] { tcp, udp };
            }
        }

        public PacketHandler packetHandle = (int i, Packet p) => { };
        public DisconectHandler disconectHandler = (int i) => {};

        public Client(int _clientId)
        {
            id = _clientId;
            tcp = new TCP(id, this);
            udp = new UDP(id, this);
        }

        public void Disconect()
        {
            if (IsConnected)
            {
                tcp.Disconect();
                udp.Disconect();
            }
        }
    }
}
