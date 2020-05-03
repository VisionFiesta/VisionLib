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

        [NetPacketHandler(NetCommand.NC_USER_CLIENT_RIGHTVERSION_CHECK_ACK, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_USER_CLIENT_RIGHTVERSION_CHECK_ACK(NetPacket _, NetClientConnection connection)
        {
            ClientLog.Debug( "Client version check passed!");
            connection.GameClient.LoginService.SetStatus(ClientLoginServiceStatus.CLSS_VERIFIED);
        }

        [NetPacketHandler(NetCommand.NC_USER_CLIENT_WRONGVERSION_CHECK_ACK, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_USER_CLIENT_WRONGVERSION_CHECK_ACK(NetPacket packet, NetClientConnection connection)
        {
            ClientLog.Debug( "Client version check failed!");
            connection.GameClient.LoginService.SetStatus(ClientLoginServiceStatus.CLSS_NOTCONNECTED);
        }

        [NetPacketHandler(NetCommand.NC_USER_LOGIN_ACK, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_USER_LOGIN_ACK(NetPacket packet, NetClientConnection connection)
        {
            ClientLog.Debug("User login succeeded!");
#if SHINE_GER
            var ack = new NcUserLoginAck();
            ack.Read(packet);

            connection.GameClient.GameData.Worlds.AddRange(ack.WorldStatuses);
            connection.GameClient.LoginService.SetStatus(ClientLoginServiceStatus.CLSS_GOTWORLDS)
#else
            connection.GameClient.LoginService.SetStatus(ClientLoginServiceStatus.CLSS_LOGGEDIN);
#endif
        }

        [NetPacketHandler(NetCommand.NC_USER_LOGINFAIL_ACK, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_USER_LOGINFAIL_ACK(NetPacket packet, NetClientConnection connection)
        {
            var err = (LoginResponse)packet.Reader.ReadUInt16();
            ClientLog.Warning("User login failed: " + err.ToMessage());
            connection.GameClient.LoginService.SetStatus(ClientLoginServiceStatus.CLSS_NOTCONNECTED);
        }

        [NetPacketHandler(NetCommand.NC_USER_XTRAP_ACK, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_USER_XTRAP_ACK(NetPacket packet, NetClientConnection connection)
        {
            var ack = packet.Reader.ReadByte();
            ClientLog.Debug( $"XTrap ACK {(ack == 1 ? "OK" : "FAIL")}");
        }

        [NetPacketHandler(NetCommand.NC_USER_WORLD_STATUS_ACK, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_USER_WORLD_STATUS_ACK(NetPacket packet, NetClientConnection connection)
        {
            var ack = new NcUserWorldStatusAck();
            ack.Read(packet);

            ClientLog.Debug( "Got world list: " + ack);

            connection.GameClient.GameData.Worlds.AddRange(ack.WorldStatuses);
            connection.GameClient.LoginService.SetStatus(ClientLoginServiceStatus.CLSS_GOTWORLDS);
        }

        [NetPacketHandler(NetCommand.NC_USER_WORLDSELECT_ACK, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_USER_WORLDSELECT_ACK(NetPacket packet, NetClientConnection connection)
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

        [NetPacketHandler(NetCommand.NC_USER_LOGINWORLD_ACK, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_USER_LOGINWORLD_ACK(NetPacket packet, NetClientConnection connection)
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

        [NetPacketHandler(NetCommand.NC_USER_LOGINWORLDFAIL_ACK, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_USER_LOGINWORLDFAIL_ACK(NetPacket packet, NetClientConnection connection)
        {
            var errorCode = new ProtoErrorcode();
            errorCode.Read(packet.Reader);

            ClientLog.Warning( "World login failed: " + errorCode.ErrorCode);

            connection.GameClient.WorldService.SetStatus(ClientWorldServiceStatus.CWSS_NOTCONNECTED);
        }

#endregion
    }
}
