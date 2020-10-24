using Vision.Core.Collections;
using Vision.Core.Extensions;
using Vision.Core.Logging.Loggers;
using Vision.Core.Networking;
using Vision.Core.Networking.Packet;
using Vision.Game.Characters;
using Vision.Game.Content;
using Vision.Game.Content.GameObjects;
using Vision.Game.Structs.BriefInfo;

namespace Vision.Client.Networking.Handlers
{
    public static class BriefInfoHandlers
    {
        private static readonly ClientLog Logger = new ClientLog(typeof(BriefInfoHandlers));

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_INFORM_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_INFORM_CMD(NetPacket packet, NetClientConnection connection)
        {
            Logger.Debug("BI_INFORM");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_CHANGEDECORATE_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_CHANGEDECORATE_CMD(NetPacket packet, NetClientConnection connection)
        {
            Logger.Debug("BI_CHANGEDECORATE");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_CHANGEUPGRADE_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_CHANGEUPGRADE_CMD(NetPacket packet, NetClientConnection connection)
        {
            Logger.Debug("BI_CHANGEUPGRADE");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_UNEQUIP_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_UNEQUIP_CMD(NetPacket packet, NetClientConnection connection)
        {
            Logger.Debug("BI_UNEQUIP");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_CHANGEWEAPON_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_CHANGEWEAPON_CMD(NetPacket packet, NetClientConnection connection)
        {
            Logger.Debug("BI_CHANGEWEAPON");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_LOGINCHARACTER_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_LOGINCHARACTER_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcBriefInfoLoginCharacterCmd();
            cmd.Read(packet);

            var chr = new Character(cmd.Handle);
            chr.Initialize(cmd);

            connection.Account.ActiveCharacter.VisibleObjects.Add(chr);
            Logger.Debug($"BI_LOGINCHARACTER: Added {chr}");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_CHARACTER_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_CHARACTER_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcBriefInfoCharacterCmd();
            cmd.Read(packet);

            var chrList = new FastList<Character>();
            foreach (var chr in cmd.Characters)
            {
                var newChr = new Character(chr.Handle);
                newChr.Initialize(chr);
                chrList.Add(newChr);
                Logger.Debug($"BI_CHARACTER: Added {newChr}");
            }

            connection.Account.ActiveCharacter.VisibleObjects.AddRange(chrList);
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_REGENMOB_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_REGENMOB_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcBriefInfoRegenMobCmd();
            cmd.Read(packet);

            var mob = new Mob(cmd);

            connection.Account.ActiveCharacter.VisibleObjects.Add(mob);
            Logger.Debug($"BI_REGENMOB: Added {mob}");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_MOB_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_MOB_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcBriefInfoMobCmd();
            cmd.Read(packet);

            var mobList = new FastList<Mob>();
            foreach (var mobRaw in cmd.Mobs)
            {
                var mob = new Mob(mobRaw);
                mobList.Add(mob);
                Logger.Debug($"BI_MOB: Added {mob}");
            }

            connection.Account.ActiveCharacter.VisibleObjects.AddRange(mobList);
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_DROPEDITEM_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_DROPEDITEM_CMD(NetPacket packet, NetClientConnection connection)
        {
            Logger.Debug("BI_DROPEDITEM");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_ITEMONFIELD_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_ITEMONFIELD_CMD(NetPacket packet, NetClientConnection connection)
        {
            Logger.Debug("BI_ITEMONFIELD");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_MAGICFIELDSPREAD_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_MAGICFIELDSPREAD_CMD(NetPacket packet, NetClientConnection connection)
        {
            Logger.Debug("BI_MAGICFIELDSPREAD");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_MAGICFIELDINFO_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_MAGICFIELDINFO_CMD(NetPacket packet, NetClientConnection connection)
        {
            Logger.Debug("BI_MAGICFIELDINFO");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_BRIEFINFODELETE_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_BRIEFINFODELETE_CMD(NetPacket packet, NetClientConnection connection)
        {
            var handle = packet.Reader.ReadUInt16();
            var go = GameObject.Objects.First(o => o.Handle == handle);
            if (go == null)
            {
                Logger.Error($"BI_DELETE: GameObject no longer present, Handle: {handle}");
                return;
            }

            var visibleObjects = connection.Account.ActiveCharacter.VisibleObjects;

            var result = visibleObjects.Remove(go);
            if (!result) result = visibleObjects.Remove(go);

            Logger.Debug(result
                ? $"BI_DELETE: Removed GameObject {go.Type.ToFriendlyName()}, Handle: {go.Handle} "
                : $"BI_DELETE: Failed to remove GameObject - Handle: {go.Handle}");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_BUILDDOOR_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_BUILDDOOR_CMD(NetPacket packet, NetClientConnection connection)
        {
            Logger.Debug("BI_BUILDDOOR");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_DOOR_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_DOOR_CMD(NetPacket packet, NetClientConnection connection)
        {
            Logger.Debug("BI_DOOR");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_EFFECTBLAST_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_EFFECTBLAST_CMD(NetPacket packet, NetClientConnection connection)
        {
            Logger.Debug("BI_EFFECTBLAST");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_EFFECT_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_EFFECT_CMD(NetPacket packet, NetClientConnection connection)
        {
            Logger.Debug("BI_EFFECT");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_MINIHOUSEBUILD_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_MINIHOUSEBUILD_CMD(NetPacket packet, NetClientConnection connection)
        {
            Logger.Debug("BI_MINIHOUSEBUILD");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_MINIHOUSE_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_MINIHOUSE_CMD(NetPacket packet, NetClientConnection connection)
        {
            Logger.Debug("BI_MINIHOUSE");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_PLAYER_LIST_INFO_APPEAR_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_PLAYER_LIST_INFO_APPEAR_CMD(NetPacket packet, NetClientConnection connection)
        {
            Logger.Debug("BI_PLAYER_LIST_INFO_APPEAR");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_ABSTATE_CHANGE_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_ABSTATE_CHANGE_CMD(NetPacket packet, NetClientConnection connection)
        {
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
            Logger.Debug("BI_ABSTATE_CHANGE");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_ABSTATE_CHANGE_LIST_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_ABSTATE_CHANGE_LIST_CMD(NetPacket packet, NetClientConnection connection)
        {
            Logger.Debug("BI_ABSTATE_CHANGE_LIST");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_REGENMOVER_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_REGENMOVER_CMD(NetPacket packet, NetClientConnection connection)
        {
            Logger.Debug("BI_REGENMOVER");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_MOVER_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_MOVER_CMD(NetPacket packet, NetClientConnection connection)
        {
            Logger.Debug("BI_MOVER");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_REGENPET_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_REGENPET_CMD(NetPacket packet, NetClientConnection connection)
        {
            Logger.Debug("BI_REGENPET");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_PET_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_PET_CMD(NetPacket packet, NetClientConnection connection)
        {
            Logger.Debug("BI_PET");
        }
    }
}
