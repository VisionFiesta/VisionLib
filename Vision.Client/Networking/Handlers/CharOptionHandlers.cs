using Vision.Core.Networking;
using Vision.Core.Networking.Packet;
using Vision.Game.Structs.CharOption;

namespace Vision.Client.Networking.Handlers
{
    public class CharOptionHandlers
    {
        [NetPacketHandler(NetCommand.NC_CHAR_OPTION_IMPROVE_GET_SHORTCUTDATA_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_CHAR_OPTION_IMPROVE_GET_SHORTCUTDATA_CMD(NetPacket packet, NetClientConnection connection)
        {
            var ack = new NcCharOptionImproveGetShortcutDataCmd();
            ack.Read(packet);

            connection.Account.ActiveCharacter.ShortcutData = ack.Data;
        }

        [NetPacketHandler(NetCommand.NC_CHAR_OPTION_IMPROVE_GET_KEYMAP_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_CHAR_OPTION_IMPROVE_GET_KEYMAP_CMD(NetPacket packet, NetClientConnection connection)
        {

        }

        [NetPacketHandler(NetCommand.NC_CHAR_OPTION_IMPROVE_GET_GAMEOPTION_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_CHAR_OPTION_IMPROVE_GET_GAMEOPTION_CMD(NetPacket packet, NetClientConnection connection)
        {

        }
    }
}
