using Vision.Core.Logging.Loggers;
using Vision.Core.Networking;
using Vision.Core.Networking.Packet;
using Vision.Game.Structs.Act;

namespace Vision.Client.Networking.Handlers
{
    public static class ActHandlers
    {
        #region Move-Related

        [FiestaNetPacketHandler(FiestaNetCommand.NC_ACT_SOMEONEMOVEWALK_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_ACT_SOMEONEMOVEWALK_CMD(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {
            // TODO
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_ACT_SOMEONEMOVERUN_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_ACT_SOMEONEMOVERUN_CMD(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {
            // TODO
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_ACT_SOMEONESTOP_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_ACT_SOMEONESTOP_CMD(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {
            // TODO
        }

        #endregion

        #region Other

        [FiestaNetPacketHandler(FiestaNetCommand.NC_ACT_SOMEONEPRODUCE_MAKE_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_ACT_SOMEONEPRODUCE_MAKE_CMD(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {
            // TODO
        }

        #endregion

        #region Chat-Related

        [FiestaNetPacketHandler(FiestaNetCommand.NC_ACT_SOMEONECHAT_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_ACT_SOMEONECHAT_CMD(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {
            var cmd = new NcActSomeoneChatCmd();
            cmd.Read(packet);
            
            connection.GameClient.ChatService.ReceiveChat(ClientLogChatType.CLCT_NORMAL, cmd.Message, cmd.Handle);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_ACT_SOMEONESHOUT_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_ACT_SOMEONESHOUT_CMD(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {
            var cmd = new NcActSomeoneShoutCmd();
            cmd.Read(packet);

            connection.GameClient.ChatService.ReceiveChat(ClientLogChatType.CLCT_SHOUT, cmd.Message, cmd.SenderName);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_ACT_SOMEONEWHISPER_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_ACT_SOMEONEWHISPER_CMD(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {
            var cmd = new NcActSomeoneWhisperCmd();
            cmd.Read(packet);

            connection.GameClient.ChatService.ReceiveChat(ClientLogChatType.CLCT_WHISPER, cmd.Message, cmd.SenderName);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_ACT_WHISPERSUCCESS_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_ACT_WHISPERSUCCESS_ACK(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {
            var ack = new NcActWhisperSuccessAck();
            ack.Read(packet);

            connection.GameClient.ChatService.SendChatAck(ClientLogChatType.CLCT_WHISPER, ack.Message, ack.ReceiverName);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_ACT_PARTYCHAT_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_ACT_PARTYCHAT_CMD(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {
            var cmd = new NcActPartyChatCmd();
            cmd.Read(packet);

            connection.GameClient.ChatService.ReceiveChat(ClientLogChatType.CLCT_PARTY, cmd.Chat.Message, cmd.SenderName);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_ACT_PARTYCHAT_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_ACT_PARTYCHAT_ACK(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {
            // ignored?
        }

        #endregion
    }
}
