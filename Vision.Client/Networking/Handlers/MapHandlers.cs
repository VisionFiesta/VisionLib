using Vision.Client.Enums;
using Vision.Core.Common.Extensions;
using Vision.Core.Logging.Loggers;
using Vision.Core.Networking;
using Vision.Core.Networking.Packet;
using Vision.Game.Content.GameObjects;

namespace Vision.Client.Networking.Handlers
{
    public static class MapHandlers
    {
        [NetPacketHandler(NetCommand.NC_MAP_LOGIN_ACK, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_MAP_LOGIN_ACK(NetPacket packet, NetClientConnection connection)
        {
            // TODO: move to ClientZoneService
            new NetPacket(NetCommand.NC_MAP_LOGINCOMPLETE_CMD).Send(connection);
            ClientLog.Info("Map Login OK");
        }

        [NetPacketHandler(NetCommand.NC_MAP_LOGINFAIL_ACK, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_MAP_LOGINFAIL_ACK(NetPacket packet, NetClientConnection connection)
        {
            ClientLog.Warning("Map Login Failed");
            connection.GameClient.WorldService.SetStatus(ClientWorldServiceStatus.CWSS_NOTCONNECTED);
            connection.Disconnect();
        }

        [NetPacketHandler(NetCommand.NC_MAP_LOGOUT_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_MAP_LOGOUT_CMD(NetPacket packet, NetClientConnection connection)
        {
            var handle = packet.Reader.ReadUInt16();

            var go = GameObject.Objects.First(o => o.Handle == handle);
            if (go == null)
            {
                ClientLog.Error($"Missing GameObject for MAP_LOGOUT! Handle: {handle}");
                return;
            }

            var result = connection.Account.ActiveCharacter.VisibleObjects.Remove(go);

            if (result)
                ClientLog.Debug($"MAP_LOGOUT_CMD: Removed GameObject - Handle: {go.Handle}");
            else
                ClientLog.Warning($"MAP_LOGOUT_CMD: Failed to remove GameObject - Handle: {go.Handle}");
        }
    }
}
