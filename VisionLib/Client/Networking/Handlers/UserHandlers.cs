using System.Diagnostics.CodeAnalysis;
using VisionLib.Client.Enums;
using VisionLib.Client.Services;
using VisionLib.Common.Enums;
using VisionLib.Common.Logging;
using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Packet;
using VisionLib.Common.Networking.Protocols.Misc;
using VisionLib.Common.Networking.Structs.Common;
using VisionLib.Common.Networking.Structs.User;

namespace VisionLib.Client.Networking.Handlers
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class UserHandlers
    {
        [FiestaNetPacketHandlerAttritube(FiestaNetCommand.NC_USER_CLIENT_RIGHTVERSION_CHECK_ACK)]
        public static void NC_USER_CLIENT_RIGHTVERSION_CHECK_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            Log.Write(LogType.GameLog, LogLevel.Debug, "Client version check passed!");
            LoginService.SetStatus(ClientLoginStatus.CLS_VERIFIED);
        }

        [FiestaNetPacketHandlerAttritube(FiestaNetCommand.NC_USER_CLIENT_WRONGVERSION_CHECK_ACK)]
        public static void NC_USER_CLIENT_WRONGVERSION_CHECK_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            Log.Write(LogType.GameLog, LogLevel.Debug, "Client version check failed!");
            LoginService.SetStatus(ClientLoginStatus.CLS_NOTCONNECTED);
        }

        [FiestaNetPacketHandlerAttritube(FiestaNetCommand.NC_USER_LOGIN_ACK)]
        public static void NC_USER_LOGIN_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            Log.Write(LogType.GameLog, LogLevel.Debug, "User login succeeded!");
            LoginService.SetStatus(ClientLoginStatus.CLS_LOGGEDIN);
        }

        [FiestaNetPacketHandlerAttritube(FiestaNetCommand.NC_USER_LOGINFAIL_ACK)]
        public static void NC_USER_LOGINFAIL_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var err = (LoginResponse)packet.ReadUInt16();
            Log.Write(LogType.GameLog, LogLevel.Warning, "User login failed: " + err.ToMessage());
            LoginService.SetStatus(ClientLoginStatus.CLS_NOTCONNECTED);
        }

        [FiestaNetPacketHandlerAttritube(FiestaNetCommand.NC_USER_XTRAP_ACK)]
        public static void NC_USER_XTRAP_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var ack = packet.ReadByte();
            Log.Write(LogType.GameLog, LogLevel.Debug, $"XTrap ACK {(ack == 1 ? "OK" : "FAIL")}");
        }

        [FiestaNetPacketHandlerAttritube(FiestaNetCommand.NC_USER_WORLD_STATUS_ACK)]
        public static void NC_USER_WORLD_STATUS_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var result = new STRUCT_NC_USER_WORLD_STATUS_ACK(packet);
            Log.Write(LogType.GameLog, LogLevel.Debug, "Got world list: " + result);

            new STRUCT_NC_USER_WORLDSELECT_REQ(0).ToPacket().Send(connection);
        }

        [FiestaNetPacketHandlerAttritube(FiestaNetCommand.NC_USER_WORLDSELECT_ACK)]
        public static void NC_USER_WORLDSELECT_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var result = new STRUCT_NC_USER_WORLDSELECT_ACK(packet);
            Log.Write(LogType.GameLog, LogLevel.Debug, "Got world select ack: " + result);
            if (result.WorldStatus.IsJoinable())
            {
                Log.Write(LogType.GameLog, LogLevel.Debug, "Connecting to world server...");

                FiestaConsoleClient.LoginData.WmTransferKey = result.ConnectionHash;
                FiestaConsoleClient.LoginData.WorldIP = result.WorldIPv4;
                FiestaConsoleClient.LoginData.WorldPort = result.WorldPort;

                FiestaConsoleClient.WorldClient.Connect(result.WorldIPv4, result.WorldPort);
            }
            else
            {
                Log.Write(LogType.GameLog, LogLevel.Warning, $"Unable to join world: {result.WorldStatus.ToMessage()}");
            }
        }

        [FiestaNetPacketHandlerAttritube(FiestaNetCommand.NC_USER_LOGINWORLD_ACK)]
        public static void NC_USER_LOGINWORLD_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var result = new STRUCT_NC_USER_LOGINWORLD_ACK(packet);

            FiestaConsoleClient.LoginData.WmHandle = result.WorldManagerHandle;

            var avatarStr = "";
            if (result.AvatarCount > 0)
            {
                foreach (var avatar in result.Avatars)
                {
                    avatarStr += "\n    ";
                    avatarStr += avatar.ToString();
                }
            }
            Log.Write(LogType.GameLog, LogLevel.Debug, $"Got world login ack. Found {result.AvatarCount} avatars." + avatarStr);

            new PROTO_NC_MISC_GAMETIME_REQ().Send(connection);
        }

        [FiestaNetPacketHandlerAttritube(FiestaNetCommand.NC_USER_LOGINWORLDFAIL_ACK)]
        public static void NC_USER_LOGINWORLDFAIL_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var message = new STRUCT_PROTO_ERRORCODE(packet);
            Log.Write(LogType.GameLog, LogLevel.Warning, "World login failed: " + message.ErrorCode);
        }
    }
}
