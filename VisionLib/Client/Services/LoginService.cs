using System;
using System.Collections.Generic;
using VisionLib.Client.Configuration;
using VisionLib.Client.Enums;
using VisionLib.Common.Logging;
using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Protocols.User;
using VisionLib.Common.Networking.Structs.User;

namespace VisionLib.Client.Services
{
    public static class LoginService
    {
        public static ClientLoginStatus LoginStatus { get; private set; } = ClientLoginStatus.CLS_NOTCONNECTED;

        private static readonly FiestaNetConnection LoginConnection = FiestaConsoleClient.LoginClient;
        private static readonly ClientConfiguration Config = FiestaConsoleClient.Config;

        private static void Update()
        {
            switch (LoginStatus)
            {
                case ClientLoginStatus.CLS_NOTCONNECTED:
                    Log.Write(LogType.GameLog, LogLevel.Info, "LoginService: Disconnected");
                    break;
                case ClientLoginStatus.CLS_TRYCONNECT:
                    LoginConnection.Connect(Config.LoginServerIP, Config.LoginServerPort);
                    Log.Write(LogType.GameLog, LogLevel.Info, "LoginService: Connecting...");
                    break;
                case ClientLoginStatus.CLS_CONNECTED:
                    Log.Write(LogType.GameLog, LogLevel.Info, "LoginService: Connected");
                    new STRUCT_NC_USER_CLIENT_VERSION_CHECK_REQ(Config.BinMD5, Config.ClientVersionData).ToPacket().Send(LoginConnection);
                    break;
                case ClientLoginStatus.CLS_VERIFIED:
                    Log.Write(LogType.GameLog, LogLevel.Info, "LoginService: Version verified");
                    new STRUCT_NC_USER_US_LOGIN_REQ(Config.FiestaUsername, Config.FiestaPassword).ToPacket().Send(LoginConnection);
                    new STRUCT_NC_USER_XTRAP_REQ((byte)Config.XTrapVersionHash.Length, Config.XTrapVersionHash).ToPacket().Send(LoginConnection);
                    break;
                case ClientLoginStatus.CLS_LOGGEDIN:
                    Log.Write(LogType.GameLog, LogLevel.Info, "LoginService: Logged in");
                    new PROTO_NC_USER_WORLD_STATUS_REQ().Send(LoginConnection);
                    break;
                case ClientLoginStatus.CLS_GOTWORLDS:
                    Log.Write(LogType.GameLog, LogLevel.Info, "LoginService: Got World list");
                    new STRUCT_NC_USER_WORLDSELECT_REQ(0).ToPacket().Send(LoginConnection);
                    break;
                case ClientLoginStatus.CLS_JOINEDWORLD:
                    Log.Write(LogType.GameLog, LogLevel.Info, "LoginService: Starting world connection./..");
                    // do nothing.
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static void SetStatus(ClientLoginStatus status)
        {
            LoginStatus = status;
            Update();
        }
    }
}
