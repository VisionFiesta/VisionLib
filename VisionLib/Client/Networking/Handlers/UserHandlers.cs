using VisionLib.Common.Logging;
using VisionLib.Common.Network.Protocols.User;
using VisionLib.Common.Networking;
using VisionLib.Client;
using VisionLib.Common.Networking.Structs.User;
using FakeClientV1.Enums;
using VisionLib.Common.Network.Structs.User;
using System.Threading;
using System;
using VisionLib.Common.Networking.Protocols.User;
using VisionLib.Common.Enums;
using VisionLib.Common.Networking.Structs.Common;

namespace VisionLib.Common.Network.Packets
{
    public static class UserHandlers
    {
        [FiestaNetPacketHandlerAttritube(FiestaNetCommand.NC_USER_CLIENT_RIGHTVERSION_CHECK_ACK)]
        public static void NC_USER_CLIENT_RIGHTVERSION_CHECK_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            Log.Write(LogType.GameLog, LogLevel.Info, "Client version check passed!");

            new STRUCT_NC_USER_US_LOGIN_REQ(FiestaClient.Config.FiestaUsername, FiestaClient.Config.FiestaPassword).ToPacket().Send(connection);
            new STRUCT_NC_USER_XTRAP_REQ((byte)FiestaClient.Config.XTrapVersionHash.Length, FiestaClient.Config.XTrapVersionHash).ToPacket().Send(connection);
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
            Log.Write(LogType.GameLog, LogLevel.Info, "Got world list! " + result.ToString());

            new PROTO_NC_USER_WORLDSELECT_REQ(0).Send(connection);
        }

        [FiestaNetPacketHandlerAttritube(FiestaNetCommand.NC_USER_WORLDSELECT_ACK)]
        public static void NC_USER_WORLDSELECT_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var result = new STRUCT_NC_USER_WORLDSELECT_ACK(packet);
            Log.Write(LogType.GameLog, LogLevel.Info, "Got world select ack! " + result.ToString());
            if (result.WorldStatus.IsJoinable())
            {
                Log.Write(LogType.GameLog, LogLevel.Info, "Joining world...");
                FiestaClient.ConnectionHash = result.ConnectionHash;
                FiestaClient.WorldClient.Connect(result.WorldIPv4, result.WorldPort);
            }
            else
            {
                Log.Write(LogType.GameLog, LogLevel.Warning, "World not joinable.");
            }
        }

        [FiestaNetPacketHandlerAttritube(FiestaNetCommand.NC_USER_LOGINWORLD_ACK)]
        public static void NC_USER_LOGINWORLD_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var result = new STRUCT_NC_USER_LOGINWORLD_ACK(packet);
            Log.Write(LogType.GameLog, LogLevel.Info, "Got world login ack!");
        }

        [FiestaNetPacketHandlerAttritube(FiestaNetCommand.NC_USER_LOGINWORLDFAIL_ACK)]
        public static void NC_USER_LOGINWORLDFAIL_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var message = new STRUCT_PROTO_ERRORCODE(packet);
            Log.Write(LogType.GameLog, LogLevel.Warning, "World login failed: " + message.err);
        }
    }
}
