using System;
using VisionLib.Common.Networking.Packet;
using VisionLib.Common.Utils;

namespace VisionLib.Common.Networking.Structs.User
{
    public class STRUCT_NC_USER_US_LOGIN_REQ : FiestaNetPacketStruct
    {
        internal const int UsernameLen = 260;
        internal const int PasswordMD5Len = 36;
        internal const int StartupAppLen = 20;

        public readonly string Username;
        public readonly string PasswordMD5;
        public readonly string SpawnApp; // NA default: "Original"

        public STRUCT_NC_USER_US_LOGIN_REQ(string username, string password, string spawnApp = "Original", bool isPassMD5d = false)
        {
            Username = username;
            PasswordMD5 = isPassMD5d ? password : MD5Utils.CreateMD5(password);
            SpawnApp = spawnApp;
        }

        public STRUCT_NC_USER_US_LOGIN_REQ(FiestaNetPacket packet)
        {
            Username = packet.ReadString(UsernameLen);
            PasswordMD5 = packet.ReadString(PasswordMD5Len);
            SpawnApp = packet.ReadString(StartupAppLen);
        }

        public override FiestaNetPacket ToPacket()
        {
            var pkt = new FiestaNetPacket(FiestaNetCommand.NC_USER_US_LOGIN_REQ);
            WriteToPacket(pkt);
            return pkt;
        }

        public override void WriteToPacket(FiestaNetPacket pkt)
        {
            pkt.Write(Username, UsernameLen);
            pkt.Write(PasswordMD5, PasswordMD5Len);
            pkt.Write(SpawnApp, StartupAppLen);
        }
    }
}
