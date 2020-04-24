using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Packet;

namespace VisionLib.Client.Networking.Handlers
{
    public static class BriefInfoHandlers
    {
        [FiestaNetPacketHandler(FiestaNetCommand.NC_BRIEFINFO_ABSTATE_CHANGE_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_BRIEFINFO_ABSTATE_CHANGE_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            // TODO:
        }
    }
}
