using Vision.Core.Networking;
using Vision.Core.Networking.Packet;
using Vision.Game.Structs.Announce;

namespace Vision.Client.Networking.Handlers
{
    public static class AnnounceHandlers
    {
        [NetPacketHandler(NetCommand.NC_ANNOUNCE_W2C_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_ANNOUNCE_W2C_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcAnnounceW2CCmd();
            cmd.Read(packet);

            // TODO: Move to ChatService
            // ClientLog.Announce(cmd.AnnounceType, cmd.Message);
        }
    }
}
