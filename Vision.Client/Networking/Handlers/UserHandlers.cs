using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Vision.Client.Enums;
using Vision.Core.Logging.Loggers;
using Vision.Core.Networking;
using Vision.Core.Networking.Packet;
using Vision.Game;
using Vision.Game.Characters.Shape;
using Vision.Game.Enums;
using Vision.Game.Structs.Common;
using Vision.Game.Structs.User;

namespace Vision.Client.Networking.Handlers
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class UserHandlers
    {
        #region LoginPackets

        [FiestaNetPacketHandler(FiestaNetCommand.NC_USER_CLIENT_RIGHTVERSION_CHECK_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_USER_CLIENT_RIGHTVERSION_CHECK_ACK(FiestaNetPacket _, FiestaNetClientConnection connection)
        {
            ClientLog.Debug( "Client version check passed!");
            connection.GameClient.LoginService.SetStatus(ClientLoginServiceStatus.CLSS_VERIFIED);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_USER_CLIENT_WRONGVERSION_CHECK_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_USER_CLIENT_WRONGVERSION_CHECK_ACK(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {
            ClientLog.Debug( "Client version check failed!");
            connection.GameClient.LoginService.SetStatus(ClientLoginServiceStatus.CLSS_NOTCONNECTED);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_USER_LOGIN_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_USER_LOGIN_ACK(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {
            ClientLog.Debug( "User login succeeded!");
            connection.GameClient.LoginService.SetStatus(ClientLoginServiceStatus.CLSS_LOGGEDIN);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_USER_LOGINFAIL_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_USER_LOGINFAIL_ACK(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {
            var err = (LoginResponse)packet.Reader.ReadUInt16();
            ClientLog.Warning("User login failed: " + err.ToMessage());
            connection.GameClient.LoginService.SetStatus(ClientLoginServiceStatus.CLSS_NOTCONNECTED);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_USER_XTRAP_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_USER_XTRAP_ACK(FiestaNetPacket packet, FiestaNetConnection _)
        {
            var ack = packet.Reader.ReadByte();
            ClientLog.Debug( $"XTrap ACK {(ack == 1 ? "OK" : "FAIL")}");
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_USER_WORLD_STATUS_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_USER_WORLD_STATUS_ACK(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {
            var ack = new NcUserWorldStatusAck();
            ack.Read(packet);

            ClientLog.Debug( "Got world list: " + ack);

            var client = connection.GameClient;
            client?.GameData.Worlds.AddRange(ack.WorldStatuses);
            client?.LoginService.SetStatus(ClientLoginServiceStatus.CLSS_GOTWORLDS);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_USER_WORLDSELECT_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_USER_WORLDSELECT_ACK(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {
            var result = new NcUserWorldSelectAck();
            result.Read(packet);

            ClientLog.Debug( "Got world select ack: " + result);
            if (result.WorldStatus.IsJoinable())
            {
                var client = connection.GameClient;
                ClientLog.Debug( "Connecting to world server...");
                client?.LoginService.SetWMTransferKey(result.ConnectionHash);
                client?.LoginService.SetWMEndpoint(result.WorldIPv4, result.WorldPort);
                client?.LoginService.SetStatus(ClientLoginServiceStatus.CLSS_JOININGWORLD);
            }
            else
            {
                ClientLog.Warning( $"Unable to join world: {result.WorldStatus.ToMessage()}");
            }
        }

        #endregion

        #region WorldPackets

        [FiestaNetPacketHandler(FiestaNetCommand.NC_USER_LOGINWORLD_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_USER_LOGINWORLD_ACK(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {
            var result = new NcUserLoginWorldAck();
            result.Read(packet);

            var avatarStr = "";
            if (result.AvatarCount > 0)
            {
                avatarStr = result.Avatars.Aggregate(avatarStr, (current, avatar) => current + $"\n    {avatar}");
            }

            connection.Account.AccountID = result.AccountID;

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
                connection.Account.Avatars.Add(trueAva);
            }
            

            ClientLog.Debug( $"Got {result.AvatarCount} avatars." + avatarStr);

            connection.GameClient.WorldService.SetStatus(ClientWorldServiceStatus.CWSS_GOTAVATARS);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_USER_LOGINWORLDFAIL_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_USER_LOGINWORLDFAIL_ACK(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {
            var errorCode = new ProtoErrorcode();
            errorCode.Read(packet.Reader);

            ClientLog.Warning( "World login failed: " + errorCode.ErrorCode);

            connection.GameClient.WorldService.SetStatus(ClientWorldServiceStatus.CWSS_NOTCONNECTED);
        }

        #endregion
    }
}
