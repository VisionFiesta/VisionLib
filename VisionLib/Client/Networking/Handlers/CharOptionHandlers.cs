using VisionLib.Common.Extensions;
using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Packet;
using VisionLib.Core.Struct.CharOption;

namespace VisionLib.Client.Networking.Handlers
{
    public class CharOptionHandlers
    {
        [FiestaNetPacketHandler(FiestaNetCommand.NC_CHAR_OPTION_IMPROVE_GET_SHORTCUTDATA_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_CHAR_OPTION_IMPROVE_GET_SHORTCUTDATA_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var ack = new NcCharOptionImproveGetShortcutDataCmd();
            ack.Read(packet);

            connection.GetAccount().ActiveCharacter.ShortcutData = ack.Data;
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_CHAR_OPTION_IMPROVE_GET_KEYMAP_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_CHAR_OPTION_IMPROVE_GET_KEYMAP_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {

        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_CHAR_OPTION_IMPROVE_GET_GAMEOPTION_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_CHAR_OPTION_IMPROVE_GET_GAMEOPTION_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {

        }
    }
}
