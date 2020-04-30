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
        [FiestaNetPacketHandler(FiestaNetCommand.NC_MAP_LOGIN_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_MAP_LOGIN_ACK(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {
            // TODO: move to ClientZoneService
            new FiestaNetPacket(FiestaNetCommand.NC_MAP_LOGINCOMPLETE_CMD).Send(connection);
            ClientLog.Info("Map Login OK");
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_MAP_LOGINFAIL_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_MAP_LOGINFAIL_ACK(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {
            ClientLog.Warning("Map Login Failed");
            connection.GameClient.WorldService.SetStatus(ClientWorldServiceStatus.CWSS_NOTCONNECTED);
            connection.Disconnect();
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_MAP_LOGOUT_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_MAP_LOGOUT_CMD(FiestaNetPacket packet, FiestaNetClientConnection connection)
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
