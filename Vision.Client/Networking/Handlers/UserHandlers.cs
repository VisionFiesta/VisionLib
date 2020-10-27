using System.Diagnostics.CodeAnalysis;
using System.Net;
using Vision.Client.Services;
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
        private static readonly ClientLog Logger = new ClientLog(typeof(UserHandlers));

        #region LoginPackets

        [NetPacketHandler(NetCommand.NC_USER_CLIENT_RIGHTVERSION_CHECK_ACK, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_USER_CLIENT_RIGHTVERSION_CHECK_ACK(NetPacket _, NetClientConnection connection)
        {
            Logger.Debug( "Client version check passed!");
            connection.UpdateLoginService(LoginServiceTrigger.LST_VERIFY_OK);
        }

        [NetPacketHandler(NetCommand.NC_USER_CLIENT_WRONGVERSION_CHECK_ACK, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_USER_CLIENT_WRONGVERSION_CHECK_ACK(NetPacket packet, NetClientConnection connection)
        {
            Logger.Warning( "Client version check failed!");
            connection.UpdateLoginService(LoginServiceTrigger.LST_VERIFY_FAIL);
        }

        [NetPacketHandler(NetCommand.NC_USER_LOGIN_ACK, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_USER_LOGIN_ACK(NetPacket packet, NetClientConnection connection)
        {
            Logger.Debug("User login succeeded!");

            connection.GameClient.ClientSessionData.ClientAccount.AccountName = connection.GameClient.UserData.Username;

            try
            {
                NC_USER_WORLD_STATUS_ACK(packet, connection);
            }
            catch
            {
                connection.UpdateLoginService(LoginServiceTrigger.LST_LOGIN_OK);
            }
        }

        [NetPacketHandler(NetCommand.NC_USER_LOGINFAIL_ACK, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_USER_LOGINFAIL_ACK(NetPacket packet, NetClientConnection connection)
        {
            var err = (LoginResponse)packet.Reader.ReadUInt16();
            Logger.Warning("User login failed: " + err.ToMessage());
            connection.UpdateLoginService(LoginServiceTrigger.LST_LOGIN_FAIL);
        }

        [NetPacketHandler(NetCommand.NC_USER_XTRAP_ACK, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_USER_XTRAP_ACK(NetPacket packet, NetClientConnection connection)
        {
            var ack = packet.Reader.ReadByte();
            Logger.Debug( $"XTrap ACK {(ack == 1 ? "OK" : "FAIL")}");
        }

        [NetPacketHandler(NetCommand.NC_USER_WORLD_STATUS_ACK, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_USER_WORLD_STATUS_ACK(NetPacket packet, NetClientConnection connection)
        {
            var ack = new NcUserWorldStatusAck();
            ack.Read(packet);

            Logger.Debug( "Got world list: " + ack);

            connection.GameClient.ClientSessionData.Worlds.AddRange(ack.WorldStatuses);
            connection.UpdateLoginService(LoginServiceTrigger.LST_GET_WORLDS_OK);
        }

        [NetPacketHandler(NetCommand.NC_USER_WORLDSELECT_ACK, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_USER_WORLDSELECT_ACK(NetPacket packet, NetClientConnection connection)
        {
            var result = new NcUserWorldSelectAck();
            result.Read(packet);

            Logger.Debug( "Got world select ack: " + result);
            if (result.WorldStatus.IsJoinable())
            {
                connection.GameClient.ClientSessionData.SelectedWorldEndPoint = new IPEndPoint(IPAddress.Parse(result.WorldIPv4), result.WorldPort);
                connection.GameClient.ClientSessionData.WorldAuthBytes = result.ConnectionHash;
                connection.UpdateLoginService(LoginServiceTrigger.LST_SELECT_WORLD_OK);
            }
            else
            {
                Logger.Warning($"Unable to select world: {result.WorldStatus.ToMessage()}");
                connection.UpdateLoginService(LoginServiceTrigger.LST_SELECT_WORLD_FAIL);
            }
        }

#endregion

        #region WorldPackets

        [NetPacketHandler(NetCommand.NC_USER_LOGINWORLD_ACK, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_USER_LOGINWORLD_ACK(NetPacket packet, NetClientConnection connection)
        {
            var result = new NcUserLoginWorldAck();
            result.Read(packet);

            // ReSharper disable once CoVariantArrayConversion
            var avatarStr = string.Join(", ", (object[])result.Avatars);

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

                if (!connection.Account.AddAvatar(trueAva))
                {
                    Logger.Error("USER_LOGINWORLD_ACK: Failed to add avatar!");
                }
            }


            Logger.Debug( $"USER_LOGINWORLD_ACK: Got {result.AvatarCount} avatars" + avatarStr);

            connection.UpdateWorldService(WorldServiceTrigger.WST_LOGIN_OK);
        }

        [NetPacketHandler(NetCommand.NC_USER_LOGINWORLDFAIL_ACK, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_USER_LOGINWORLDFAIL_ACK(NetPacket packet, NetClientConnection connection)
        {
            var errorCode = new ProtoErrorcode();
            errorCode.Read(packet.Reader);

            Logger.Warning( "World login failed: " + errorCode.ErrorCode);

            connection.UpdateWorldService(WorldServiceTrigger.WST_LOGIN_FAIL);
        }

        #endregion
    }
}
