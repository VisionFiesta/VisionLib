using Vision.Core.Logging.Loggers;
using Vision.Core.Networking;
using Vision.Core.Networking.Packet;
using Vision.Game.Enums;
using Vision.Game.Structs.Announce;

namespace Vision.Client.Networking.Handlers
{
    public static class AnnounceHandlers
    {
        [NetPacketHandler(NetCommand.NC_ANNOUNCE_W2C_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_ANNOUNCE_W2C_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcAnnounceW2CCmd();
            cmd.Read(packet);

            var chatType = cmd.AnnounceType == AnnounceType.AT_ROAR
                ? ClientLogChatType.CLCT_ROAR
                : ClientLogChatType.CLCT_ANNOUNCE;

            connection.GameClient.ChatService.ReceiveChat(chatType, cmd.Message, cmd.AnnounceType.ToFriendlyName());
        }
    }
}
