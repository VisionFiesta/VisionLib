using System;
using System.Collections.Generic;
using System.Text;
using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Packet;

namespace VisionLib.Client.Networking.Handlers
{
    public static class FriendHandlers
    {
        [FiestaNetPacketHandlerAttritube(FiestaNetCommand.NC_FRIEND_SET_CONFIRM_REQ)]
        public static void NC_FRIEND_SET_CONFIRM_REQ(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var receiver = packet.ReadString(20); // me
            var sender = packet.ReadString(20); // person adding
            var pkt = new FiestaNetPacket(FiestaNetCommand.NC_FRIEND_SET_CONFIRM_ACK);
            pkt.Write(receiver, 20); // me
            pkt.Write(sender, 20); // person adding
            pkt.Write(1); // 1 = yes, 0 = no
            pkt.Send(connection);
        }
    }
}
