﻿using Vision.Client.Services;
using Vision.Core.Extensions;
using Vision.Core.Logging.Loggers;
using Vision.Core.Networking;
using Vision.Core.Networking.Packet;
using Vision.Game.Content.GameObjects;

namespace Vision.Client.Networking.Handlers
{
    public static class MapHandlers
    {
        private static readonly ClientLog Logger = new ClientLog(typeof(MapHandlers));

        [NetPacketHandler(NetCommand.NC_MAP_LOGIN_ACK, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_MAP_LOGIN_ACK(NetPacket packet, NetClientConnection connection)
        {
            // TODO: move to ClientZoneService
            Logger.Info("Map Login OK");
            connection.UpdateWorldService(WorldServiceTrigger.WST_LOGIN_ZONE_OK);
            // new NetPacket(NetCommand.NC_MAP_LOGINCOMPLETE_CMD).Send(connection);
        }

        [NetPacketHandler(NetCommand.NC_MAP_LOGINFAIL_ACK, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_MAP_LOGINFAIL_ACK(NetPacket packet, NetClientConnection connection)
        {
            Logger.Warning("Map Login Failed");
            // TODO: zone or world service?
            connection.UpdateWorldService(WorldServiceTrigger.WST_LOGIN_ZONE_FAIL);
            // connection.Disconnect();
        }

        [NetPacketHandler(NetCommand.NC_MAP_LOGOUT_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_MAP_LOGOUT_CMD(NetPacket packet, NetClientConnection connection)
        {
            // var handle = packet.Reader.ReadUInt16();

            // var go = GameObject.Objects.First(o => o.Handle == handle);
            // if (go == null)
            // {
            //     Logger.Error($"Missing GameObject for MAP_LOGOUT! Handle: {handle}");
            //     return;
            // }
            //
            // var result = connection.Account.ActiveCharacter.VisibleObjects.Remove(go);
            //
            // if (result)
            //     Logger.Debug($"MAP_LOGOUT_CMD: Removed GameObject - Handle: {go.Handle}");
            // else
            //     Logger.Warning($"MAP_LOGOUT_CMD: Failed to remove GameObject - Handle: {go.Handle}");
        }
    }
}
