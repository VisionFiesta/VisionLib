using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Packet;

namespace VisionLib.Client.Networking.Handlers
{
    public static class BriefInfoHandlers
    {
        [FiestaNetPacketHandler(FiestaNetCommand.NC_BRIEFINFO_UNEQUIP_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_BRIEFINFO_UNEQUIP_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            // TODO:
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_BRIEFINFO_ABSTATE_CHANGE_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_BRIEFINFO_ABSTATE_CHANGE_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            // TODO:

            /*
             * using(ScriptAPI)
             * {
	         *      AddUShort("handle");
	         *      //ABSTATE_INFORMATION info
	         *      //->ABSTATEINDEX abstateID (uint)
	         *      //->uint restKeepTime
	         *      //->uint strength
	         *      AddUInt("abstateID");
	         *      AddUInt("restKeepTime");
	         *      AddUInt("strength");
             * }
             *
             */
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_BRIEFINFO_ABSTATE_CHANGE_LIST_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_BRIEFINFO_ABSTATE_CHANGE_LIST_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            // TODO: 
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_BRIEFINFO_CHARACTER_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_BRIEFINFO_CHARACTER_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {

        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_BRIEFINFO_MOB_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_BRIEFINFO_MOB_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {

        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_BRIEFINFO_MOVER_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_BRIEFINFO_MOVER_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {

        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_BRIEFINFO_BRIEFINFODELETE_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_BRIEFINFO_BRIEFINFODELETE_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {

        }
    }
}
