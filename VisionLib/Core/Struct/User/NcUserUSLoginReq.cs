﻿using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Packet;
using VisionLib.Common.Utils;
using VisionLib.Core.Stream;

namespace VisionLib.Core.Struct.User
{
    public class NcUserUSLoginReq : NetPacketStruct
    {
        internal const int UsernameLen = 260;
        internal const int PasswordMD5Len = 36;
        internal const int StartupAppLen = 20;

        public string Username { get; private set; }
        public string PasswordMD5 { get; private set; }
        public string SpawnApp { get; private set; } // NA default: "Original"

        public NcUserUSLoginReq(string username, string password, string spawnApp = "Original", bool isPassPlaintext = true)
        {
            Username = username;
            PasswordMD5 = isPassPlaintext ? MD5Utils.CreateMD5(password) : password;
            SpawnApp = spawnApp;
        }

        public override FiestaNetPacket ToPacket()
        {
            var pkt = new FiestaNetPacket(FiestaNetCommand.NC_USER_US_LOGIN_REQ);
            Write(pkt.Writer);
            return pkt;
        }

        public override int GetSize()
        {
            return UsernameLen + PasswordMD5Len + StartupAppLen;
        }

        public override void Read(ReaderStream reader)
        {
            Username = reader.ReadString(UsernameLen);
            PasswordMD5 = reader.ReadString(PasswordMD5Len);
            SpawnApp = reader.ReadString(StartupAppLen);
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(Username, UsernameLen);
            writer.Write(PasswordMD5, PasswordMD5Len);
            writer.Write(SpawnApp, StartupAppLen);
        }
    }
}