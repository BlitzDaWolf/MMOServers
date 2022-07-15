using Algo;
using NetCommen.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetCommen.NetworkClient
{
    public class UDP : INetworkClient
    {
        public string ip = "127.0.0.1";
        public int port = 7575;

        private Client c;

        public UdpClient socket;
        public IPEndPoint endPoint;

        public bool IsServer = false;

        public int id;

        public SendData sendData = (IPEndPoint endPoint, Packet _packet) => { };

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

            sendData = HandleSend;

            Packet pkt = new Packet();
            pkt.WriteLength();
            SendData(pkt);
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

        public void SendData(Packet packet)
        {
            if (packet.EncryptFlag)
            {
                byte[] before = packet.ReadBytes(5);
                byte[] result = Encryption.Instance.Encrypt(packet.ReadBytes(packet.UnreadLength()));
                packet = new Packet(before);
                packet.Write(result);

                Packet _packetData = new Packet(packet.ToArray());

                before = _packetData.ReadBytes(5);
                byte[] mid = _packetData.ReadBytes(_packetData.UnreadLength());
            }
            if (endPoint != null)
                sendData(endPoint, packet);
        }

        public void SendData(IPackage packet)
        {
            Packet pkt = (IsServer) ? packet.ServerPack() : packet.ClientPack();
            pkt.WriteLength();
            SendData(pkt);
        }

        public void HandleData(byte[] data)
        {
            Packet packet = new Packet(data);
            HandleData(packet);
        }

        public void HandleData(Packet _packetData)
        {
            int _packetLength = _packetData.ReadInt();
            bool encrypted = _packetData.ReadBool();

            if (encrypted)
            {
                _packetData = new Packet(_packetData.ToArray());

                int skip = (IsServer) ? 9 : 5;

                List<byte> before = _packetData.ReadBytes(skip).ToList();
                int toRead = _packetData.UnreadLength();

                byte[] mid = _packetData.ReadBytes(toRead, false);
                byte[] after = Decryption.Instance.Decrypt(mid);

                before.AddRange(after);
                _packetData = new Packet(before.ToArray());
                _packetData.ReadBytes(skip);
            }

            // _packetData = (encrypted) ? new Packet(_packetData.ReadBytes(_packetData.UnreadLength())) : _packetData;
            _packetData.EncryptFlag = encrypted;

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
}
