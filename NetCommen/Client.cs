using NetCommen.Interface;
using System.Net;
using System.Net.Sockets;

namespace NetCommen
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
        public static DisconectHandler disconectHandler = (int i) => {};

        public Client(int _clientId)
        {
            id = _clientId;
            tcp = new TCP(id, this);
            udp = new UDP(id, this);
        }

        public class TCP : INetworkClient
        {
            private readonly Client c;
            public bool IsServer { get; set; } = false;

            public TcpClient socket;
            public int id;
            private NetworkStream stream;
            private Packet receivedData;
            private byte[] receiveBuffer;
            public bool connected { get; private set; } = false;

            public int challange { get; private set; }

            public TCP(int _id, Client c)
            {
                id = _id;
                this.c = c;
            }

            public void Connect(string ip, int port)
            {
                socket = new TcpClient
                {
                    ReceiveBufferSize = dataBufferSize,
                    SendBufferSize = dataBufferSize,
                };

                connected = true;
                IsServer = false;

                receiveBuffer  = new byte[dataBufferSize];
                receivedData = new Packet();

                socket.BeginConnect(ip, port, ConnectCallback, socket);
            }

            public void Connect(TcpClient _socket)
            {
                socket = _socket;
                socket.ReceiveBufferSize = dataBufferSize;

                connected = true;
                IsServer = true;

                stream = socket.GetStream();
                receiveBuffer  = new byte[dataBufferSize];

                receivedData = new Packet();
                stream.BeginRead(receiveBuffer , 0, dataBufferSize, ReciveCallback, null);
            }

            public void WriteHandshake()
            {
                challange = NetworkCallbacks.createChallange();
                Packet pkt = new Packet(NETWORK_COMMANDS.SC_Handshake);
                pkt.Write(NETWORK_COMMANDS.NETWORK_VERSION);
                pkt.Write(challange);
                pkt.Write(id);
                pkt.WriteLength();
                SendData(pkt);
            }

            private void ConnectCallback(IAsyncResult _result)
            {
                socket.EndConnect(_result);

                if (!socket.Connected)
                    return;

                stream = socket.GetStream();
                receivedData = new Packet();

                NetworkCallbacks.Conncted();
                stream.BeginRead(receiveBuffer , 0, receiveBuffer .Length, ReciveCallback, null);
            }

            public void Disconect()
            {
                socket.Close();
                stream = null;
                receivedData = null;
                receivedData = null;
                socket = null;

                connected = false;
            }

            public void Disconect(bool reason, string msg)
            {
                Packet pkt = new Packet(NETWORK_COMMANDS.SC_Disconect);
                pkt.Write(reason);
                pkt.Write(msg);
                pkt.WriteLength();
                SendData(pkt);

                socket.Close();
                stream = null;
                receivedData = null;
                receiveBuffer = null;
                socket = null;
            }

            public void SendData(Packet packet)
            {
                try
                {
                    if (socket != null)
                    {
                        stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            public void SendData(IPackage packet)
            {
                Packet pkt = (IsServer) ? packet.ServerPack() : packet.ClientPack();
                pkt.WriteLength();
                SendData(pkt);
            }

            private void ReciveCallback(IAsyncResult _result)
            {
                try
                {
                    int _byteLength = stream.EndRead(_result);
                    if (_byteLength <= 0)
                    {
                        disconectHandler(id);
                        return;
                    }

                    byte[] _data = new byte[_byteLength];
                    Array.Copy(receiveBuffer , _data, _byteLength);

                    receivedData.Reset(HandleData(_data));
                    stream.BeginRead(receiveBuffer , 0, dataBufferSize, ReciveCallback, null);
                }
                catch (Exception e)
                {
                    disconectHandler(id);
                }
            }

            private bool HandleData(byte[] _data)
            {
                int _packetLength = 0;
                receivedData.SetBytes(_data);

                if (receivedData.UnreadLength() >= 4)
                {
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }

                while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
                {
                    byte[] _packetbytes = receivedData.ReadBytes(_packetLength);
                    Packet _packet = new Packet(_packetbytes);
                    c.packetHandle(id, _packet);
                }

                _packetLength = 0;
                if (receivedData.UnreadLength() >= 4)
                {
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }


                if (receivedData.UnreadLength() <= 1)
                {
                    return true;
                }

                return false;
            }
        }

        public class UDP : INetworkClient
        {

            public string ip = "127.0.0.1";
            public int port = 7575;

            private Client c;

            public UdpClient socket;
            public IPEndPoint endPoint;

            public bool IsServer = false;

            public int id;

            public UDP(int _id, Client c)
            {
                this.c = c;
                id = _id;
            }

            public void HandleSend(IPEndPoint endPoint, Packet packet)
            {
                try
                {
                    packet.InsertInt(id);
                    if (socket != null)
                    {
                        socket.BeginSend(packet.ToArray(), packet.Length(), null, null);
                    }
                }
                catch (Exception)
                {

                }
            }

            public void Connect(IPEndPoint _endPoint)
            {
                IsServer = true;
                endPoint = _endPoint;
                NetworkCallbacks.OnClientConnected(c);
            }

            public void Connect(int _localPort)
            {
                IsServer = false;
                endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                socket = new UdpClient(_localPort);

                socket.Connect(endPoint);
                socket.BeginReceive(ReceiveCallback, null);

                NetworkCallbacks.sendData = HandleSend;

                using (Packet pkt = new Packet())
                {
                    SendData(pkt);
                }
            }

            private void ReceiveCallback(IAsyncResult _result)
            {
                byte[] _data = socket.EndReceive(_result, ref endPoint);
                socket.BeginReceive(ReceiveCallback, null);

                if (_data.Length < 4)
                {
                    // TODO: disconnect
                    return;
                }

                HandleData(_data);
            }

            public void SendData(Packet _packet)
            {
                if (endPoint != null)
                    NetworkCallbacks.sendData(endPoint, _packet);
            }

            public void SendData(IPackage packet)
            {
                Packet pkt = (IsServer) ? packet.ServerPack() : packet.ClientPack();
                pkt.WriteLength();
                SendData(pkt);
            }

            public void HandleData(byte[] data)
            {
                using (Packet packet = new Packet(data))
                {
                    HandleData(packet);
                }
            }

            public void HandleData(Packet _packetData)
            {
                int _packetLength = _packetData.ReadInt();
                c.packetHandle(id, _packetData);
            }

            public void Disconect()
            {
                endPoint = null;
            }

            public void Disconect(bool reason, string msg)
            {

            }
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
