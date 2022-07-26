using Algo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetCommen;
using NetCommen.Interface;
using NetCommen.NetworkClient;
using System.Net;
using System.Net.Sockets;

namespace MainServer
{
    public class Network
    {
        protected readonly IConfiguration configuration;
        public PacketHandler packetHandle = (int i, Packet p) => { };

        public TcpListener? Listener;
        public ILogger<Network> Logger;

        private Dictionary<int, Client> clients = new Dictionary<int, Client>();
        private int MaxPlayers = 25;
        private int Port = 7575;

        private TcpListener TcpListener;
        private UdpClient UdpListener;
        public DisconectHandler disconectHandler = (int i) => { };

        public OnClientConnected Connected = (Client c) => { };

        public int ConnectedClients
        {
            get
            {
                return clients.Values.Count(x => x.IsConnected);
            }
        }

        public Network(ILogger<Network> logger, IConfiguration configuration)
        {
            Logger = logger;
            this.configuration = configuration;
            MaxPlayers = configuration.GetValue<int>("server:maxplayers");
            Port = configuration.GetValue<int>("server:port");

            // sendData = SendUDPData;

            InitiliazeClients();
        }

        public Network(ILogger<Network> logger, IConfiguration configuration, Encryption encryption, Decryption decryption) : this(logger, configuration)
        {
        }

        /// <summary>
        /// Gets a spesific client
        /// </summary>
        /// <param name="clientId">Client to lookup</param>
        /// <returns>Found client</returns>
        public Client? GetClient(int clientId)
        {
            if (clients.ContainsKey(clientId))
            {
                Client c = clients[clientId];
                if (c.tcp.connected)
                {
                    return c;
                }
                Logger.LogInformation($"Client({clientId}) is not connected");
                return null;
            }
            Logger.LogInformation($"Client({clientId}) was not found");
            return null;
        }

        /// <summary>
        /// Sends a package to all the connected clients
        /// </summary>
        /// <param name="pkt">package to send</param>
        /// <param name="protocol">TCP, UDP</param>
        /// <param name="ignore">Wich client to ignore</param>
        public void SendToAllClients(IPackage pkt, int protocol = 0, int ignore = -1)
        {
            foreach (var client in clients.Values)
            {
                if (client.tcp.connected && client.id != ignore)
                {
                    client.NetworkClient[protocol].SendData(pkt);
                }
            }
        }

        /// <summary>
        /// Sends a Packet to all the connected clients
        /// </summary>
        /// <param name="pkt">Packet to send</param>
        /// <param name="protocol">TCP, UDP</param>
        /// <param name="ignore">Wich client to ignore</param>
        public void SendToAllClients(Packet pkt, int protocol = 0, int ignore = -1)
        {
            foreach (var client in clients.Values)
            {
                if (client.tcp.connected && client.id != ignore)
                {
                    client.NetworkClient[protocol].SendData(pkt);
                }
            }
        }

        /// <summary>
        /// Handles the disconection from a client
        /// </summary>
        /// <param name="clientId">Disconected client</param>
        private void PlayerDisconect(int clientId)
        {
            if(clientId < 0)
            {
                Logger.LogInformation($"The event server has been closed.");
                return;
            }
            else
            {
                disconectHandler(clientId);
                Logger.LogInformation($"Client({clientId}) disconected");

                clients[clientId].Disconect();

                Packet pkt = new Packet(NETWORK_COMMANDS.SC_PlayerDisconect);
                pkt.Write(clientId);
                pkt.WriteLength();
                SendToAllClients(pkt, 0, clientId);
            }
        }

        /// <summary>
        /// Starts up the networking
        /// </summary>
        public void Start()
        {
            TcpListener = new TcpListener(IPAddress.Any, Port);
            TcpListener.Start();
            TcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);

            UdpListener = new UdpClient(Port);
            UdpListener.BeginReceive(UDPReceiveCallback, null);

            Logger.LogInformation($"Server started on port {Port}");
        }

        /// <summary>
        /// Send a UDP packet to a endpoint
        /// </summary>
        /// <param name="_clientEndPoint">To who does the package needs to be send</param>
        /// <param name="_packet">Packet to send</param>
        public void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
        {
            try
            {
                if (_clientEndPoint != null)
                {
                    UdpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
                }
            }
            catch (Exception _ex)
            {
                Console.WriteLine($"Error sending data to {_clientEndPoint} via UDP: {_ex}");
            }
        }

        /// <summary>
        /// Arival of UDP packets
        /// </summary>
        private void UDPReceiveCallback(IAsyncResult result)
        {
            try
            {
                IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] _data = UdpListener.EndReceive(result, ref _clientEndPoint);
                UdpListener.BeginReceive(UDPReceiveCallback, null);

                if (_data.Length < 4)
                {
                    return;
                }

                Packet _packet = new Packet(_data);
                {
                    int _clientId = _packet.ReadInt();

                    if (_clientId == 0)
                    {
                        return;
                    }

                    if (clients[_clientId].udp.endPoint == null)
                    {
                        // If this is a new connection
                        clients[_clientId].udp.Connect(_clientEndPoint);
                        Client c = GetClient(_clientId);
                        Connected(c);
                        return;
                    }

                    if (clients[_clientId].udp.endPoint.ToString() == _clientEndPoint.ToString())
                    {
                        // Ensures that the client is not being impersonated by another by sending a false clientID
                        clients[_clientId].udp.HandleData(_packet);
                    }
                }
            }
            catch (Exception _ex)
            {
                Console.WriteLine($"Error receiving UDP data: {_ex}");
            }
        }

        private void TCPConnectCallback(IAsyncResult result)
        {
            TcpClient client = TcpListener.EndAcceptTcpClient(result);
            TcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);
            // Logger.LogInformation($"Incoming connection from {client.Client.RemoteEndPoint}...");

            for (int i = 1; i <= MaxPlayers; i++)
            {
                if (clients[i].tcp.socket == null)
                {
                    clients[i].tcp.Connect(client);
                    return;
                }
            }

            Logger.LogInformation($"{client.Client.RemoteEndPoint} failed to connect: Server full!");
            client.Close();
        }

        /// <summary>
        /// Create all clients
        /// </summary>
        private void InitiliazeClients()
        {
            for (int i = 1; i <= MaxPlayers; i++)
            {
                clients.Add(i, new Client(i)
                {
                    packetHandle = (int i, Packet p) => packetHandle(i, p),
                    disconectHandler = (int i) => PlayerDisconect(i)
                });
                clients[i].udp.sendData = SendUDPData;
            }
        }
    }
}
