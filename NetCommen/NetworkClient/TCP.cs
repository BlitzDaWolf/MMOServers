using NetCommen.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetCommen.NetworkClient
{
    public class TCP : INetworkClient
    {
        public const int dataBufferSize = 4096;

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

            receiveBuffer = new byte[dataBufferSize];
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
            receiveBuffer = new byte[dataBufferSize];

            receivedData = new Packet();
            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReciveCallback, null);
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

            c.Conncted();
            stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, ReciveCallback, null);
        }

        public void Disconect()
        {
            if (socket != null)
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
                    c.disconectHandler(id);
                    return;
                }

                byte[] _data = new byte[_byteLength];
                Array.Copy(receiveBuffer, _data, _byteLength);

                receivedData.Reset(HandleData(_data));
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReciveCallback, null);
            }
            catch (Exception e)
            {
                c.disconectHandler(id);
            }
        }

        private bool HandleData(byte[] _data)
        {
            try
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
            }
            catch
            {

            }

            return false;
        }
    }
}
