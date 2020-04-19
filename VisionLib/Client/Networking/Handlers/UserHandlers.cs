using System.Diagnostics.CodeAnalysis;
using VisionLib.Client.Configuration;
using VisionLib.Common.Enums;
using VisionLib.Common.Logging;
using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Protocols.Misc;
using VisionLib.Common.Networking.Protocols.User;
using VisionLib.Common.Networking.Structs.Common;
using VisionLib.Common.Networking.Structs.User;

namespace VisionLib.Client.Networking.Handlers
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class UserHandlers
    {
        private static readonly ClientConfiguration Config = FiestaClient.Config;

        [FiestaNetPacketHandlerAttritube(FiestaNetCommand.NC_USER_CLIENT_RIGHTVERSION_CHECK_ACK)]
        public static void NC_USER_CLIENT_RIGHTVERSION_CHECK_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            Log.Write(LogType.GameLog, LogLevel.Info, "Client version check passed!");

            new STRUCT_NC_USER_US_LOGIN_REQ(Config.FiestaUsername, Config.FiestaPassword).ToPacket().Send(connection);
            new STRUCT_NC_USER_XTRAP_REQ((byte)Config.XTrapVersionHash.Length, Config.XTrapVersionHash).ToPacket().Send(connection);
        }

        [FiestaNetPacketHandlerAttritube(FiestaNetCommand.NC_USER_CLIENT_WRONGVERSION_CHECK_ACK)]
        public static void NC_USER_CLIENT_WRONGVERSION_CHECK_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            Log.Write(LogType.GameLog, LogLevel.Warning, "Client version check failed!");
        }

        [FiestaNetPacketHandlerAttritube(FiestaNetCommand.NC_USER_LOGIN_ACK)]
        public static void NC_USER_LOGIN_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            Log.Write(LogType.GameLog, LogLevel.Info, "User login succeeded!");
            new PROTO_NC_USER_WORLD_STATUS_REQ().Send(connection);
        }

        [FiestaNetPacketHandlerAttritube(FiestaNetCommand.NC_USER_LOGINFAIL_ACK)]
        public static void NC_USER_LOGINFAIL_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var err = (LoginResponse)packet.ReadUInt16();
            Log.Write(LogType.GameLog, LogLevel.Warning, "User login failed: " + err.ToMessage());
        }

        [FiestaNetPacketHandlerAttritube(FiestaNetCommand.NC_USER_XTRAP_ACK)]
        public static void NC_USER_XTRAP_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var ack = packet.ReadByte();
            Log.Write(LogType.GameLog, LogLevel.Info, $"XTrap ACK {(ack == 1 ? "OK" : "FAIL")}");
        }

        [FiestaNetPacketHandlerAttritube(FiestaNetCommand.NC_USER_WORLD_STATUS_ACK)]
        public static void NC_USER_WORLD_STATUS_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var result = new STRUCT_NC_USER_WORLD_STATUS_ACK(packet);
            Log.Write(LogType.GameLog, LogLevel.Info, "Got world list: " + result);

            new PROTO_NC_USER_WORLDSELECT_REQ(0).Send(connection);
        }

        [FiestaNetPacketHandlerAttritube(FiestaNetCommand.NC_USER_WORLDSELECT_ACK)]
        public static void NC_USER_WORLDSELECT_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var result = new STRUCT_NC_USER_WORLDSELECT_ACK(packet);
            Log.Write(LogType.GameLog, LogLevel.Info, "Got world select ack: " + result);
            if (result.WorldStatus.IsJoinable())
            {
                Log.Write(LogType.GameLog, LogLevel.Info, "Connecting to world server...");
                FiestaClient.LoginData.WmTransferKey = result.ConnectionHash;
                FiestaClient.WorldClient.Connect(result.WorldIPv4, result.WorldPort);
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
            var avatarStr = "";
            if (result.AvatarCount > 0)
            {
                foreach (var avatar in result.Avatars)
                {
                    avatarStr += "\n    ";
                    avatarStr += avatar.ToString();
                }
            }
            Log.Write(LogType.GameLog, LogLevel.Info, $"Got world login ack. Found {result.AvatarCount} avatars." + avatarStr);

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
