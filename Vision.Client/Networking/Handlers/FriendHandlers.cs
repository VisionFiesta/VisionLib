using System.Diagnostics.CodeAnalysis;
using Vision.Core.Logging.Loggers;
using Vision.Core.Networking;
using Vision.Core.Networking.Packet;
using Vision.Game.Structs.Friend;

namespace Vision.Client.Networking.Handlers
{
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class FriendHandlers
    {
        private static readonly ClientLog Logger = new(typeof(FriendHandlers));

        [NetPacketHandler(NetCommand.NC_FRIEND_SET_ACK, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_FRIEND_SET_ACK(NetPacket packet, NetClientConnection connection)
        {
            var ack = new NcFriendSetAck();
            ack.Read(packet);

            Logger.Debug($"FRIEND_SET_ACK: {ack}");
        }

        [NetPacketHandler(NetCommand.NC_FRIEND_SET_CONFIRM_REQ, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_FRIEND_SET_CONFIRM_REQ(NetPacket packet, NetClientConnection connection)
        {
            var req = new NcFriendSetConfirmReq();
            req.Read(packet);

            var ack = new NcFriendSetConfirmAck()
            {
                RequesterCharID = req.RequesterCharID,
                ReceiverCharID = req.ReceiverCharID,
                Accept = true
            };
            ack.Send(connection);

            Logger.Info($"FRIEND_SET_CONFIRM_REQ: Adding {req.RequesterCharID} as friend!");
        }

        [NetPacketHandler(NetCommand.NC_FRIEND_DEL_ACK, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_FRIEND_DEL_ACK(NetPacket packet, NetClientConnection connection)
        {
            var ack = new NcFriendDelAck();
            ack.Read(packet);

            Logger.Debug($"FRIEND_DEL_ACK {ack}");
        }


        [NetPacketHandler(NetCommand.NC_FRIEND_LIST_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_FRIEND_LIST_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcFriendListCmd();
            cmd.Read(packet);

            Logger.Debug($"FRIEND_LIST_CMD: {cmd}");
        }

        [NetPacketHandler(NetCommand.NC_FRIEND_ADD_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_FRIEND_ADD_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcFriendAddCmd();
            cmd.Read(packet);

            Logger.Debug($"FRIEND_ADD_CMD: {cmd}");
        }

        [NetPacketHandler(NetCommand.NC_FRIEND_LOGIN_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_FRIEND_LOGIN_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcFriendLoginCmd();
            cmd.Read(packet);

            Logger.Debug($"FRIEND_LOGIN_CMD: {cmd}");
        }

        [NetPacketHandler(NetCommand.NC_FRIEND_LOGOUT_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_FRIEND_LOGOUT_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcFriendLogoutCmd();
            cmd.Read(packet);

            Logger.Debug($"FRIEND_LOGOUT_CMD: {cmd}");
        }

        [NetPacketHandler(NetCommand.NC_FRIEND_REFUSE_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_FRIEND_REFUSE_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcFriendRefuseCmd();
            cmd.Read(packet);

            Logger.Debug($"FRIEND_REFUSE_CMD: {cmd}");
        }

        [NetPacketHandler(NetCommand.NC_FRIEND_DEL_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_FRIEND_DEL_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcFriendDelCmd();
            cmd.Read(packet);

            Logger.Debug($"FRIEND_DEL_CMD: {cmd}");
        }

        [NetPacketHandler(NetCommand.NC_FRIEND_MAP_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_FRIEND_MAP_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcFriendMapCmd();
            cmd.Read(packet);

            Logger.Debug($"FRIEND_MAP_CMD: {cmd}");
        }

        [NetPacketHandler(NetCommand.NC_FRIEND_PARTY_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_FRIEND_PARTY_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcFriendPartyCmd();
            cmd.Read(packet);

            Logger.Debug($"FRIEND_PARTY_CMD: {cmd}");
        }

        [NetPacketHandler(NetCommand.NC_FRIEND_LEVEL_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_FRIEND_LEVEL_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcFriendLevelCmd();
            cmd.Read(packet);

            Logger.Debug($"FRIEND_LEVEL_CMD: {cmd}");
        }

        [NetPacketHandler(NetCommand.NC_FRIEND_CLASS_CHANGE_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_FRIEND_CLASS_CHANGE_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcFriendClassChangeCmd();
            cmd.Read(packet);

            Logger.Debug($"FRIEND_CLASS_CHANGE_CMD: {cmd}");
        }

        [NetPacketHandler(NetCommand.NC_FRIEND_POINT_ACK, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_FRIEND_POINT_ACK(NetPacket packet, NetClientConnection connection)
        {
            var ack = new NcFriendPointAck();
            ack.Read(packet);

            Logger.Debug($"FRIEND_POINT_ACK: {ack}");
        }

        [NetPacketHandler(NetCommand.NC_FRIEND_FIND_FRIENDS_ACK, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_FRIEND_FIND_FRIENDS_ACK(NetPacket packet, NetClientConnection connection)
        {
            var ack = new NcFriendFindFriendsAck();
            ack.Read(packet);

            Logger.Debug($"FRIEND_FIND_FRIENDS_ACK: {ack}");
        }

        [NetPacketHandler(NetCommand.NC_FRIEND_UES_FRIEND_POINT_ACK, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_FRIEND_UES_FRIEND_POINT_ACK(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcFriendUseFriendPointAck();
            cmd.Read(packet);

            Logger.Debug($"FRIEND_USE_FRIEND_POINT_ACK: {cmd}");
        }
    }
}
