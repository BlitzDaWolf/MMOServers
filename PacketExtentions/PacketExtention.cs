using NetCommen;
using System.Numerics;

namespace PacketExtentions
{
    public static class PacketExtention
    {
        public static void Write(this Packet packet, Vector2 vector)
        {
            packet.Write(vector.X);
            packet.Write(vector.Y);
        }
        public static void Write(this Packet packet, Vector3 vector)
        {
            packet.Write(vector.X);
            packet.Write(vector.Y);
            packet.Write(vector.Z);
        }
        public static void Write(this Packet packet, Vector4 vector)
        {
            packet.Write(vector.X);
            packet.Write(vector.Y);
            packet.Write(vector.Z);
            packet.Write(vector.W);
        }

        public static Vector2 ReadVector2(this Packet packet)
        {
            return new Vector2(packet.ReadFloat(), packet.ReadFloat());
        }
        public static Vector3 ReadVector3(this Packet packet)
        {
            return new Vector3(packet.ReadFloat(), packet.ReadFloat(), packet.ReadFloat());
        }
        public static Vector4 ReadVector4(this Packet packet)
        {
            return new Vector4(packet.ReadFloat(), packet.ReadFloat(), packet.ReadFloat(), packet.ReadFloat());
        }

        public static void Write(this Packet packet, Quaternion quaternion)
        {
            packet.Write(quaternion.X);
            packet.Write(quaternion.Y);
            packet.Write(quaternion.Z);
            packet.Write(quaternion.W);
        }
        public static Quaternion ReadQuaternion(this Packet packet)
        {
            return new Quaternion(packet.ReadFloat(), packet.ReadFloat(), packet.ReadFloat(), packet.ReadFloat());
        }
    }
}
