using System;
using System.Collections.Generic;
using System.Text;
using VisionLib.Common.Networking.Packet;

namespace VisionLib.Common.Networking.Handlers
{
    public static class MiscHeartbeatHandler
    {
        private static readonly FiestaNetPacket HeartbeatAck = new FiestaNetPacket(FiestaNetCommand.NC_MISC_HEARTBEAT_ACK);

        [FiestaNetPacketHandlerAttritube(FiestaNetCommand.NC_MISC_HEARTBEAT_REQ)]
        public static void NC_MISC_HEARTBEAT_REQUEST(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            HeartbeatAck.Send(connection);
        }
    }
}
