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
using VisionLib.Core.Struct.Common;
using VisionLib.Core.Struct.User;

namespace VisionLib.Client.Networking.Handlers
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class UserHandlers
    {
        #region LoginPackets

        [FiestaNetPacketHandler(FiestaNetCommand.NC_USER_CLIENT_RIGHTVERSION_CHECK_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_USER_CLIENT_RIGHTVERSION_CHECK_ACK(FiestaNetPacket _, FiestaNetConnection connection)
        {
            Log.Write(LogType.GameLog, LogLevel.Debug, "Client version check passed!");
            connection.GetClient()?.LoginService.SetStatus(ClientLoginServiceStatus.CLSS_VERIFIED);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_USER_CLIENT_WRONGVERSION_CHECK_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_USER_CLIENT_WRONGVERSION_CHECK_ACK(FiestaNetPacket _, FiestaNetConnection connection)
        {
            Log.Write(LogType.GameLog, LogLevel.Debug, "Client version check failed!");
            connection.GetClient()?.LoginService.SetStatus(ClientLoginServiceStatus.CLSS_NOTCONNECTED);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_USER_LOGIN_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_USER_LOGIN_ACK(FiestaNetPacket _, FiestaNetConnection connection)
        {
            Log.Write(LogType.GameLog, LogLevel.Debug, "User login succeeded!");
            connection.GetClient()?.LoginService.SetStatus(ClientLoginServiceStatus.CLSS_LOGGEDIN);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_USER_LOGINFAIL_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_USER_LOGINFAIL_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var err = (LoginResponse)packet.Reader.ReadUInt16();
            Log.Write(LogType.GameLog, LogLevel.Warning, "User login failed: " + err.ToMessage());
            connection.GetClient()?.LoginService.SetStatus(ClientLoginServiceStatus.CLSS_NOTCONNECTED);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_USER_XTRAP_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_USER_XTRAP_ACK(FiestaNetPacket packet, FiestaNetConnection _)
        {
            var ack = packet.Reader.ReadByte();
            Log.Write(LogType.GameLog, LogLevel.Debug, $"XTrap ACK {(ack == 1 ? "OK" : "FAIL")}");
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_USER_WORLD_STATUS_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_USER_WORLD_STATUS_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var ack = new NcUserWorldStatusAck();
            ack.Read(packet.Reader);

            Log.Write(LogType.GameLog, LogLevel.Debug, "Got world list: " + ack);

            var client = connection.GetClient();
            client?.GameData.Worlds.AddRange(ack.WorldStatuses);
            client?.LoginService.SetStatus(ClientLoginServiceStatus.CLSS_GOTWORLDS);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_USER_WORLDSELECT_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_USER_WORLDSELECT_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var result = new NcUserWorldSelectAck();
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
            var result = new NcUserLoginWorldAck();
            result.Read(packet.Reader);

            connection.GetClient()?.WorldService.SetWMHandle(result.WorldManagerHandle);
            
            var avatarStr = "";
            if (result.AvatarCount > 0)
            {
                avatarStr = result.Avatars.Aggregate(avatarStr, (current, avatar) => current + $"\n    {avatar}");
            }

            foreach (var ava in result.Avatars)
            {
                var trueAva = new Avatar(ava.CharNo)
                {
                    DeleteTime = ava.DeleteInfo.Time,
                    // TODO: Equipment
                    IsDeleted = ava.DeleteInfo.IsDeleted,
                    KQDate = ava.KQDate.ToDateTime(),
                    KQHandle = ava.KQHandle,
                    KQMapIndx = ava.KQMapName,
                    KQPosition = ava.nKQCoord,
                    Level = (byte)ava.Level,
                    MapIndx = ava.LoginMap,
                    Name = ava.CharName,
                    Shape = new CharacterShape(ava.CharShape),
                    Slot = ava.CharSlot,
                    TutorialState = ava.TutorialInfo
                };
                connection.GetClient()?.GameData.ClientAccount.Avatars.Add(trueAva);
            }
            

            Log.Write(LogType.GameLog, LogLevel.Debug, $"Got {result.AvatarCount} avatars." + avatarStr);

            connection.GetClient()?.WorldService.SetStatus(ClientWorldServiceStatus.CWSS_GOTAVATARS);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_USER_LOGINWORLDFAIL_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_USER_LOGINWORLDFAIL_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var errorCode = new ProtoErrorcode();
            errorCode.Read(packet.Reader);

            Log.Write(LogType.GameLog, LogLevel.Warning, "World login failed: " + errorCode.ErrorCode);

            connection.GetClient()?.WorldService.SetStatus(ClientWorldServiceStatus.CWSS_NOTCONNECTED);
        }

        #endregion
    }
}
