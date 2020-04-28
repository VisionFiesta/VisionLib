using VisionLib.Common.Extensions;
using VisionLib.Common.Logging;
using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Packet;
using VisionLib.Core.Struct.Act;

namespace VisionLib.Client.Networking.Handlers
{
    public static class ActHandlers
    {
        [FiestaNetPacketHandler(FiestaNetCommand.NC_ACT_SOMEONEMOVEWALK_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_ACT_SOMEONEMOVEWALK_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            // TODO
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_ACT_SOMEONEMOVERUN_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_ACT_SOMEONEMOVERUN_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            // TODO
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_ACT_SOMEONESTOP_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_ACT_SOMEONESTOP_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            // TODO
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_ACT_SOMEONEPRODUCE_MAKE_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_ACT_SOMEONEPRODUCE_MAKE_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            // TODO
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_ACT_SOMEONECHAT_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_ACT_SOMEONECHAT_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var cmd = new NcActSomeoneChatCmd();
            cmd.Read(packet);

            // TODO: MapObjects
            // normal chat refers to the handle of the sender for the name
            // BRIEFINFO_LOGINCHAR_CMD will give us this
            // MAP_LOGOUT_CMD should remove a character from the MapObject list

            // var senderName = ((Character)connection.GetClient().GameData.NearbyMapObjects.First(mo => mo.handle == cmd.Handle)).CharName;
            var senderName = "Unknown";
            ClientLog.Chat(ClientLogChatType.CLCT_NORMAL, senderName, cmd.Content);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_ACT_SOMEONESHOUT_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_ACT_SOMEONESHOUT_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var cmd = new NcActSomeoneShoutCmd();
            cmd.Read(packet);

            ClientLog.Chat(ClientLogChatType.CLCT_SHOUT, cmd.SenderName, cmd.Message);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_ACT_SOMEONEWHISPER_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_ACT_SOMEONEWHISPER_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var cmd = new NcActSomeoneWhisperCmd();
            cmd.Read(packet);

            ClientLog.Chat(ClientLogChatType.CLCT_WHISPER, cmd.SenderName, cmd.Message);

            var replyMessage = $"'{cmd.Message}' to you too!";
            var whisperReq = new NcActWhisperReq(cmd.SenderName, replyMessage);
            whisperReq.Send(connection.GetClient()?.ZoneClient);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_ACT_WHISPERSUCCESS_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_ACT_WHISPERSUCCESS_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var ack = new NcActWhisperSuccessAck();
            ack.Read(packet);

            ClientLog.Chat(ClientLogChatType.CLCT_WHISPER, ack.ReceiverName, ack.Message, true);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_ACT_PARTYCHAT_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_ACT_PARTYCHAT_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var cmd = new NcActPartyChatCmd();
            cmd.Read(packet);

            ClientLog.Chat(ClientLogChatType.CLCT_PARTY, cmd.SenderName, cmd.Chat.Message);
        }
    }
}
