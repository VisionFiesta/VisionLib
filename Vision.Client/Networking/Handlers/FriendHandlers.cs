using Vision.Core.Networking;
using Vision.Core.Networking.Packet;

namespace Vision.Client.Networking.Handlers
{
    public static class FriendHandlers
    {
        [NetPacketHandler(NetCommand.NC_FRIEND_SET_CONFIRM_REQ, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_FRIEND_SET_CONFIRM_REQ(NetPacket packet, NetClientConnection connection)
        {
            var receiver = packet.Reader.ReadString(20); // me
            var sender = packet.Reader.ReadString(20); // person adding
            var pkt = new NetPacket(NetCommand.NC_FRIEND_SET_CONFIRM_ACK);
            pkt.Writer.Write(receiver, 20); // me
            pkt.Writer.Write(sender, 20); // person adding
            pkt.Writer.Write(1); // 1 = yes, 0 = no
            pkt.Send(connection);
        }
    }
}
