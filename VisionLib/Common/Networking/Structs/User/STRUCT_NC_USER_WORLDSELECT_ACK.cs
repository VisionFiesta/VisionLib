using VisionLib.Common.Enums;

namespace VisionLib.Common.Networking.Structs.User
{
    public class STRUCT_NC_USER_WORLDSELECT_ACK : FiestaNetStruct
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
            pkt.Write((byte)WorldStatus);
            return pkt;
        }

        public override string ToString() => $"\tStatus: {WorldStatus.ToMessage()}, IP: {WorldIPv4}, Port: {WorldPort}";
    }
}
