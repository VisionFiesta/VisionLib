using Vision.Core.Networking;
using Vision.Core.Networking.Packet;
using Vision.Game.Structs.CharOption;

namespace Vision.Client.Networking.Handlers
{
    public class CharOptionHandlers
    {
        [FiestaNetPacketHandler(FiestaNetCommand.NC_CHAR_OPTION_IMPROVE_GET_SHORTCUTDATA_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_CHAR_OPTION_IMPROVE_GET_SHORTCUTDATA_CMD(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {
            var ack = new NcCharOptionImproveGetShortcutDataCmd();
            ack.Read(packet);

            connection.Account.ActiveCharacter.ShortcutData = ack.Data;
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_CHAR_OPTION_IMPROVE_GET_KEYMAP_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_CHAR_OPTION_IMPROVE_GET_KEYMAP_CMD(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {

        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_CHAR_OPTION_IMPROVE_GET_GAMEOPTION_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_CHAR_OPTION_IMPROVE_GET_GAMEOPTION_CMD(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {

        }
    }
}
