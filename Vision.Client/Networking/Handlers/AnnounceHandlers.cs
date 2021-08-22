using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Vision.Core.Enums;
using Vision.Core.Networking;
using Vision.Core.Networking.Packet;
using Vision.Game.Enums;
using Vision.Game.Structs.Announce;

namespace Vision.Client.Networking.Handlers
{
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
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

            string sender;
            string message;
            if (chatType == ClientLogChatType.CLCT_ROAR)
            {
                var split = cmd.Message.Split(":").ToList();
                sender = split[0];
                message = split.GetRange(1, split.Count - 1).Aggregate((s, s1) => s + s1);
            }
            else
            {
                sender = cmd.AnnounceType.ToFriendlyName();
                message = cmd.Message;
            }

            connection.GameClient.ChatService.ReceiveChat(chatType, message, sender);
        }
    }
}
