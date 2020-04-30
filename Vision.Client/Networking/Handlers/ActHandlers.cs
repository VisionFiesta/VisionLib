using Vision.Core.Logging.Loggers;
using Vision.Core.Networking;
using Vision.Core.Networking.Packet;
using Vision.Game.Structs.Act;

namespace Vision.Client.Networking.Handlers
{
    public static class ActHandlers
    {
        #region Move-Related

        [NetPacketHandler(NetCommand.NC_ACT_SOMEONEMOVEWALK_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_ACT_SOMEONEMOVEWALK_CMD(NetPacket packet, NetClientConnection connection)
        {
            // TODO
        }

        [NetPacketHandler(NetCommand.NC_ACT_SOMEONEMOVERUN_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_ACT_SOMEONEMOVERUN_CMD(NetPacket packet, NetClientConnection connection)
        {
            // TODO
        }

        [NetPacketHandler(NetCommand.NC_ACT_SOMEONESTOP_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_ACT_SOMEONESTOP_CMD(NetPacket packet, NetClientConnection connection)
        {
            // TODO
        }

        #endregion

        #region Other

        [NetPacketHandler(NetCommand.NC_ACT_SOMEONEPRODUCE_MAKE_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_ACT_SOMEONEPRODUCE_MAKE_CMD(NetPacket packet, NetClientConnection connection)
        {
            // TODO
        }

        #endregion

        #region Chat-Related

        [NetPacketHandler(NetCommand.NC_ACT_SOMEONECHAT_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_ACT_SOMEONECHAT_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcActSomeoneChatCmd();
            cmd.Read(packet);
            
            connection.GameClient.ChatService.ReceiveChat(ClientLogChatType.CLCT_NORMAL, cmd.Message, cmd.Handle);
        }

        [NetPacketHandler(NetCommand.NC_ACT_SOMEONESHOUT_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_ACT_SOMEONESHOUT_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcActSomeoneShoutCmd();
            cmd.Read(packet);

            connection.GameClient.ChatService.ReceiveChat(ClientLogChatType.CLCT_SHOUT, cmd.Message, cmd.SenderName);
        }

        [NetPacketHandler(NetCommand.NC_ACT_SOMEONEWHISPER_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_ACT_SOMEONEWHISPER_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcActSomeoneWhisperCmd();
            cmd.Read(packet);

            connection.GameClient.ChatService.ReceiveChat(ClientLogChatType.CLCT_WHISPER, cmd.Message, cmd.SenderName);
        }

        [NetPacketHandler(NetCommand.NC_ACT_WHISPERSUCCESS_ACK, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_ACT_WHISPERSUCCESS_ACK(NetPacket packet, NetClientConnection connection)
        {
            var ack = new NcActWhisperSuccessAck();
            ack.Read(packet);

            connection.GameClient.ChatService.SendChatAck(ClientLogChatType.CLCT_WHISPER, ack.Message, ack.ReceiverName);
        }

        [NetPacketHandler(NetCommand.NC_ACT_PARTYCHAT_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_ACT_PARTYCHAT_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcActPartyChatCmd();
            cmd.Read(packet);

            connection.GameClient.ChatService.ReceiveChat(ClientLogChatType.CLCT_PARTY, cmd.Chat.Message, cmd.SenderName);
        }

        [NetPacketHandler(NetCommand.NC_ACT_PARTYCHAT_ACK, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_ACT_PARTYCHAT_ACK(NetPacket packet, NetClientConnection connection)
        {
            // ignored?
        }

        #endregion
    }
}
