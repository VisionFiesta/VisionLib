using System;
using System.Collections.Generic;
using System.Text;

namespace VisionLib.Common.Networking.Structs.User
{
    public class STRUCT_NC_USER_LOGINWORLD_REQ : FiestaNetStruct
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
            pkt.Write(Username, UsernameLen);
            pkt.Write(ConnectionHash, ConnectionHashLen);
            return pkt;
        }
    }
}
