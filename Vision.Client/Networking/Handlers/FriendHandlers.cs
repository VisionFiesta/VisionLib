using Vision.Core.Networking;
using Vision.Core.Networking.Packet;

namespace Vision.Client.Networking.Handlers
{
    public static class FriendHandlers
    {
        [FiestaNetPacketHandler(FiestaNetCommand.NC_FRIEND_SET_CONFIRM_REQ, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_FRIEND_SET_CONFIRM_REQ(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {
            var receiver = packet.Reader.ReadString(20); // me
            var sender = packet.Reader.ReadString(20); // person adding
            var pkt = new FiestaNetPacket(FiestaNetCommand.NC_FRIEND_SET_CONFIRM_ACK);
            pkt.Writer.Write(receiver, 20); // me
            pkt.Writer.Write(sender, 20); // person adding
            pkt.Writer.Write(1); // 1 = yes, 0 = no
            pkt.Send(connection);
        }
    }
}
