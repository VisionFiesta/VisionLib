using VisionLib.Common.Enums;
using VisionLib.Common.Networking.Packet;

namespace VisionLib.Common.Networking.Structs.User
{
    public class STRUCT_NC_USER_WORLDSELECT_ACK : FiestaNetPacketStruct
    {
        public const int WorldIPv4Len = 16;
        public const int ConnectionHashLen = 64;

        public readonly WorldServerStatus WorldStatus;
        public readonly string WorldIPv4;
        public readonly ushort WorldPort;
        public readonly byte[] ConnectionHash;

        public STRUCT_NC_USER_WORLDSELECT_ACK(byte worldStatus, string worldIPv4, ushort worldPort, byte[] connectionHash)
        {
            WorldStatus = (WorldServerStatus)worldStatus;
            WorldIPv4 = worldIPv4;
            WorldPort = worldPort;
            ConnectionHash = connectionHash;
        }

        public STRUCT_NC_USER_WORLDSELECT_ACK(FiestaNetPacket packet)
        {
            WorldStatus = (WorldServerStatus)packet.ReadByte();
            WorldIPv4 = packet.ReadString(WorldIPv4Len);
            WorldPort = packet.ReadUInt16();
            ConnectionHash = packet.ReadBytes(ConnectionHashLen);
        }

        public override FiestaNetPacket ToPacket()
        {
            var pkt = new FiestaNetPacket(FiestaNetCommand.NC_USER_WORLDSELECT_ACK);
            WriteToPacket(pkt);
            return pkt;
        }

        public override void WriteToPacket(FiestaNetPacket pkt)
        {
            if (pkt == null) return;
            pkt.Write((byte)WorldStatus);
            pkt.Write(WorldIPv4, WorldIPv4Len);
            pkt.Write(WorldPort);
            pkt.Write(ConnectionHash, ConnectionHashLen);
        }

        public override string ToString() => $"\tStatus: {WorldStatus.ToMessage()}, IP: {WorldIPv4}, Port: {WorldPort}";
    }
}
