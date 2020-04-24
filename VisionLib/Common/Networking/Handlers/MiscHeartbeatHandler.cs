using VisionLib.Common.Networking.Packet;

namespace VisionLib.Common.Networking.Handlers
{
    public static class MiscHeartbeatHandler
    {
        private static readonly FiestaNetPacket HeartbeatAck = new FiestaNetPacket(FiestaNetCommand.NC_MISC_HEARTBEAT_ACK);

        [FiestaNetPacketHandler(FiestaNetCommand.NC_MISC_HEARTBEAT_REQ, FiestaNetConnDest.FNCDEST_CLIENT, FiestaNetConnDest.FNCDEST_LOGIN, FiestaNetConnDest.FNCDEST_WORLDMANAGER, FiestaNetConnDest.FNCDEST_ZONE)]
        public static void NC_MISC_HEARTBEAT_REQ(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            HeartbeatAck.Send(connection);
        }
    }
}
