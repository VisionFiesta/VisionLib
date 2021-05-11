using Vision.Core.Logging.Loggers;
using Vision.Core.Networking;
using Vision.Core.Networking.Packet;
using Vision.Game.Structs.Mover;

namespace Vision.Client.Networking.Handlers
{
    public static class MoverHandlers
    {
        private static readonly ClientLog Logger = new(typeof(MoverHandlers));

        [NetPacketHandler(NetCommand.NC_MOVER_SOMEONE_RIDE_ON_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_MOVER_SOMEONE_RIDE_ON_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcMoverSomeoneRideOnCmd();
            cmd.Read(packet);

            Logger.Debug($"MOVER_SOMEONE_RIDE_ON_CMD: {cmd}");
        }

        [NetPacketHandler(NetCommand.NC_MOVER_SOMEONE_RIDE_OFF_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_MOVER_SOMEONE_RIDE_OFF_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcMoverSomeoneRideOffCmd();
            cmd.Read(packet);

            Logger.Debug($"MOVER_SOMEONE_RIDE_OFF_CMD: {cmd}");
        }
    }
}
