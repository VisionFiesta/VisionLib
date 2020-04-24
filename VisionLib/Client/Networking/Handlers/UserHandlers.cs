using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using VisionLib.Client.Enums;
using VisionLib.Common.Enums;
using VisionLib.Common.Extensions;
using VisionLib.Common.Game;
using VisionLib.Common.Game.Characters.Shape;
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
        #region LoginPackets

        [FiestaNetPacketHandler(FiestaNetCommand.NC_USER_CLIENT_RIGHTVERSION_CHECK_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_USER_CLIENT_RIGHTVERSION_CHECK_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            Log.Write(LogType.GameLog, LogLevel.Debug, "Client version check passed!");
            connection.GetClient()?.LoginService.SetStatus(ClientLoginServiceStatus.CLSS_VERIFIED);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_USER_CLIENT_WRONGVERSION_CHECK_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_USER_CLIENT_WRONGVERSION_CHECK_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            Log.Write(LogType.GameLog, LogLevel.Debug, "Client version check failed!");
            connection.GetClient()?.LoginService.SetStatus(ClientLoginServiceStatus.CLSS_NOTCONNECTED);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_USER_LOGIN_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_USER_LOGIN_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            Log.Write(LogType.GameLog, LogLevel.Debug, "User login succeeded!");
            connection.GetClient()?.LoginService.SetStatus(ClientLoginServiceStatus.CLSS_LOGGEDIN);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_USER_LOGINFAIL_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_USER_LOGINFAIL_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var err = (LoginResponse)packet.ReadUInt16();
            Log.Write(LogType.GameLog, LogLevel.Warning, "User login failed: " + err.ToMessage());
            connection.GetClient()?.LoginService.SetStatus(ClientLoginServiceStatus.CLSS_NOTCONNECTED);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_USER_XTRAP_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_USER_XTRAP_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var ack = packet.ReadByte();
            // TODO: Add stage in ClientLoginService
            Log.Write(LogType.GameLog, LogLevel.Debug, $"XTrap ACK {(ack == 1 ? "OK" : "FAIL")}");
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_USER_WORLD_STATUS_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_USER_WORLD_STATUS_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var result = new STRUCT_NC_USER_WORLD_STATUS_ACK(packet);
            Log.Write(LogType.GameLog, LogLevel.Debug, "Got world list: " + result);

            var client = connection.GetClient();
            client?.GameData.Worlds.AddRange(result.WorldStatuses);
            client?.LoginService.SetStatus(ClientLoginServiceStatus.CLSS_GOTWORLDS);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_USER_WORLDSELECT_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_USER_WORLDSELECT_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var result = new STRUCT_NC_USER_WORLDSELECT_ACK(packet);
            Log.Write(LogType.GameLog, LogLevel.Debug, "Got world select ack: " + result);
            if (result.WorldStatus.IsJoinable())
            {
                var client = connection.GetClient();
                Log.Write(LogType.GameLog, LogLevel.Debug, "Connecting to world server...");
                client?.LoginService.SetWMTransferKey(result.ConnectionHash);
                client?.LoginService.SetWMEndpoint(result.WorldIPv4, result.WorldPort);
                client?.LoginService.SetStatus(ClientLoginServiceStatus.CLSS_JOININGWORLD);
            }
            else
            {
                Log.Write(LogType.GameLog, LogLevel.Warning, $"Unable to join world: {result.WorldStatus.ToMessage()}");
            }
        }

        #endregion

        #region WorldPackets

        [FiestaNetPacketHandler(FiestaNetCommand.NC_USER_LOGINWORLD_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_USER_LOGINWORLD_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var result = new STRUCT_NC_USER_LOGINWORLD_ACK(packet);

            connection.GetClient()?.WorldService.SetWMHandle(result.WorldManagerHandle);
            
            var avatarStr = "";
            if (result.AvatarCount > 0)
            {
                avatarStr = result.Avatars.Aggregate(avatarStr, (current, avatar) => current + $"\n    {avatar}");
            }

            foreach (ProtoAvatarInformation ava in result.Avatars)
            {
                var trueAva = new Avatar(ava.chrregnum)
                {
                    DeleteTime = ava.delinfo.Time,
                    // TODO: Equipment
                    IsDeleted = ava.delinfo.IsDeleted,
                    KQDate = new DateTime(ava.dKQDate._bf0), // TODO: correct?
                    KQHandle = ava.nKQHandle,
                    KQMapIndx = ava.sKQMapName,
                    KQPosition = ava.nKQCoord,
                    Level = (byte)ava.level,
                    MapIndx = ava.loginmap,
                    Name = ava.name,
                    Shape = new CharacterShape(ava.shape),
                    Slot = ava.slot,
                    TutorialState = ava.TutorialInfo
                };
                connection.GetClient()?.GameData.ClientAccount.Avatars.Add(trueAva);
            }
            
            // connection.GetClient()?.GameData.ClientAccount.Characters.Add(result.Avatars);

            Log.Write(LogType.GameLog, LogLevel.Debug, $"Got {result.AvatarCount} avatars." + avatarStr);

            connection.GetClient()?.WorldService.SetStatus(ClientWorldServiceStatus.CWSS_GOTAVATARS);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_USER_LOGINWORLDFAIL_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_USER_LOGINWORLDFAIL_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var message = new ProtoErrorcode(packet);
            Log.Write(LogType.GameLog, LogLevel.Warning, "World login failed: " + message.ErrorCode);

            connection.GetClient()?.WorldService.SetStatus(ClientWorldServiceStatus.CWSS_NOTCONNECTED);
        }

        #endregion
    }
}
