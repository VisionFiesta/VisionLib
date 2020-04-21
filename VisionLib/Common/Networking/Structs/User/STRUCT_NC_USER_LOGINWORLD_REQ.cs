using VisionLib.Common.Networking.Packet;

namespace VisionLib.Common.Networking.Structs.User
{
    public class STRUCT_NC_USER_LOGINWORLD_REQ : FiestaNetPacketStruct
    {
        public const int UsernameLen = 256;
        public const int ConnectionHashLen = 64;

        public readonly string Username;
        public readonly byte[] ConnectionHash;

        public STRUCT_NC_USER_LOGINWORLD_REQ(string username, byte[] connectionHash)
        {
            Username = username;
            ConnectionHash = connectionHash;
        }

        public STRUCT_NC_USER_LOGINWORLD_REQ(FiestaNetPacket packet)
        {
            Username = packet.ReadString(UsernameLen);
            ConnectionHash = packet.ReadBytes(ConnectionHashLen);
        }

        public override FiestaNetPacket ToPacket()
        {
            var pkt = new FiestaNetPacket(FiestaNetCommand.NC_USER_LOGINWORLD_REQ);
            WriteToPacket(pkt);
            return pkt;
        }

        public override void WriteToPacket(FiestaNetPacket pkt)
        {
            pkt.Write(Username, UsernameLen);
            pkt.Write(ConnectionHash, ConnectionHashLen);
        }
    }
}
