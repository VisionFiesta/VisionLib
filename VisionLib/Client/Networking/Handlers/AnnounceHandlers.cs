using VisionLib.Common.Logging;
using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Packet;
using VisionLib.Core.Struct.Announce;

namespace VisionLib.Client.Networking.Handlers
{
    public static class AnnounceHandlers
    {
        [FiestaNetPacketHandler(FiestaNetCommand.NC_ANNOUNCE_W2C_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_ANNOUNCE_W2C_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var cmd = new NcAnnounceW2CCmd();
            cmd.Read(packet);

            ClientLog.Announce(cmd.AnnounceType, cmd.Message);
        }
    }
}
