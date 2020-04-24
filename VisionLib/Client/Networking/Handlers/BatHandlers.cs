using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Packet;

namespace VisionLib.Client.Networking.Handlers
{
    public static class BatHandlers
    {
        [FiestaNetPacketHandler(FiestaNetCommand.NC_BAT_ABSTATESET_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_BAT_ABSTATESET_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            // TODO
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_BAT_DOTDAMAGE_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_BAT_DOTDAMAGE_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            // TODO
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_BAT_CEASE_FIRE_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_BAT_CEASE_FIRE_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            // TODO
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_BAT_ABSTATEINFORM_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_BAT_ABSTATEINFORM_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            // TODO
        }
    }
}
