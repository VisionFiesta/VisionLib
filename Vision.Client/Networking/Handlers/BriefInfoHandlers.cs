using System.Linq;
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
            var cmd = new NcBriefInfoInformCmd();
            cmd.Read(packet);

            Logger.Debug($"BI_INFORM {cmd}");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_CHANGEDECORATE_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_CHANGEDECORATE_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcBriefInfoChangeDecorateCmd();
            cmd.Read(packet);

            Logger.Debug($"BI_CHANGEDECORATE {cmd}");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_CHANGEUPGRADE_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_CHANGEUPGRADE_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcBriefInfoChangeUpgradeCmd();
            cmd.Read(packet);

            Logger.Debug($"BI_CHANGEUPGRADE: {cmd}");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_UNEQUIP_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_UNEQUIP_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcBriefInfoUnequipCmd();
            cmd.Read(packet);

            Logger.Debug($"BI_UNEQUIP: {cmd}");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_CHANGEWEAPON_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_CHANGEWEAPON_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcBriefInfoChangeWeaponCmd();
            cmd.Read(packet);

            Logger.Debug($"BI_CHANGEWEAPON: {cmd}");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_LOGINCHARACTER_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_LOGINCHARACTER_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcBriefInfoLoginCharacterCmd();
            cmd.Read(packet);

            var chr = new Character(cmd.Handle);
            chr.Initialize(cmd);

            if (connection.Account.ActiveCharacter.VisibleObjects.Add(chr))
            {
                Logger.Debug($"BI_LOGINCHARACTER: Added {chr}");
            }
            else
            {
                Logger.Error($"BI_LOGINCHARACTER: Failed to add character - Handle: {chr.Handle}");
            }
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_CHARACTER_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_CHARACTER_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcBriefInfoCharacterCmd();
            cmd.Read(packet);

            foreach (var chr in cmd.Characters)
            {
                var newChr = new Character(chr.Handle);
                newChr.Initialize(chr);
                if (connection.Account.ActiveCharacter.VisibleObjects.Add(newChr))
                {
                    Logger.Debug($"BI_CHARACTER: Added {newChr}");
                }
                else
                {
                    Logger.Error($"BI_CHARACTER: Failed to add character - Handle: {newChr.Handle}");
                }
            }
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_REGENMOB_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_REGENMOB_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcBriefInfoRegenMobCmd();
            cmd.Read(packet);

            var mob = new Mob(cmd);

            if (connection.Account.ActiveCharacter.VisibleObjects.Add(mob)) {
                Logger.Debug($"BI_REGENMOB: Added - {mob}");
            }
            else
            {
                Logger.Error($"BI_REGENMOB: Failed to add mob - Handle: {mob.Handle} already present");
            }
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_MOB_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_MOB_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcBriefInfoMobCmd();
            cmd.Read(packet);

            foreach (var mobRaw in cmd.Mobs)
            {
                var mob = new Mob(mobRaw);

                if (connection.Account.ActiveCharacter.VisibleObjects.Add(mob))
                {
                    Logger.Debug($"BI_MOB: Added {mob}");
                }
                else
                {
                    Logger.Error($"BI_MOB: Failed to add mob - Handle: {mob.Handle}");
                }
            }
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_DROPEDITEM_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_DROPEDITEM_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcBriefInfoDropedItemCmd();
            cmd.Read(packet);

            Logger.Debug($"BI_DROPEDITEM: {cmd}");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_ITEMONFIELD_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_ITEMONFIELD_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcBriefInfoItemOnFieldCmd();
            cmd.Read(packet);

            Logger.Debug($"BI_ITEMONFIELD {cmd}");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_MAGICFIELDSPREAD_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_MAGICFIELDSPREAD_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcBriefInfoMagicFieldSpreadCmd();
            cmd.Read(packet);

            Logger.Debug($"BI_MAGICFIELDSPREAD {cmd}");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_MAGICFIELDINFO_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_MAGICFIELDINFO_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcBriefInfoMagicFieldInfoCmd();
            cmd.Read(packet);

            Logger.Debug($"BI_MAGICFIELDINFO {cmd}");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_BRIEFINFODELETE_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_BRIEFINFODELETE_CMD(NetPacket packet, NetClientConnection connection)
        {
            var handle = packet.Reader.ReadUInt16();
            var gameObjectToRemove = GameObject.GameObjects.FirstOrDefault(o => o.Handle == handle);
            if (gameObjectToRemove == null)
            {
                Logger.Error($"BI_DELETE: GameObject no longer present, Handle: {handle}");
                return;
            }

            var visibleObjects = connection.Account.ActiveCharacter.VisibleObjects;

            var result = visibleObjects.Remove(gameObjectToRemove);
            if (!result) result = visibleObjects.RemoveWhere(g => g.Handle == handle) > 0;

            var toPrint = $"BI_DELETE: {(result ? "Removed" : "Failed to remove")} GameObject {gameObjectToRemove.Type.ToFriendlyName()}, Handle: {gameObjectToRemove.Handle}";

            if (result) Logger.Debug(toPrint);
            else Logger.Error(toPrint);

            var matchingHandleVOCount = visibleObjects.Count(o => o.Handle == handle);
            if (matchingHandleVOCount > 0) Logger.Error($"VOs still has {matchingHandleVOCount} GOs with matching handle!");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_BUILDDOOR_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_BUILDDOOR_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcBriefInfoBuildDoorCmd();
            cmd.Read(packet);

            Logger.Debug($"BI_BUILDDOOR: {cmd}");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_DOOR_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_DOOR_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcBriefInfoDoorCmd();
            cmd.Read(packet);

            Logger.Debug($"BI_DOOR {cmd}");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_EFFECTBLAST_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_EFFECTBLAST_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcBriefInfoEffectBlastCmd();
            cmd.Read(packet);

            Logger.Debug($"BI_EFFECTBLAST {cmd}");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_EFFECT_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_EFFECT_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcBriefInfoEffectCmd();
            cmd.Read(packet);

            Logger.Debug($"BI_EFFECT {cmd}");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_MINIHOUSEBUILD_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_MINIHOUSEBUILD_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcBriefInfoMinihouseBuildCmd();
            cmd.Read(packet);

            Logger.Debug($"BI_MINIHOUSEBUILD {cmd}");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_MINIHOUSE_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_MINIHOUSE_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcBriefInfoMinihouseCmd();
            cmd.Read(packet);

            Logger.Debug($"BI_MINIHOUSE {cmd}");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_PLAYER_LIST_INFO_APPEAR_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_PLAYER_LIST_INFO_APPEAR_CMD(NetPacket packet, NetClientConnection connection)
        {
            // Missing base struct BI_PLAYER_INFO_APPEAR
            Logger.Debug("BI_PLAYER_LIST_INFO_APPEAR");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_ABSTATE_CHANGE_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_ABSTATE_CHANGE_CMD(NetPacket packet, NetClientConnection connection)
        {

            // Logger.Debug("BI_ABSTATE_CHANGE");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_ABSTATE_CHANGE_LIST_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_ABSTATE_CHANGE_LIST_CMD(NetPacket packet, NetClientConnection connection)
        {
            Logger.Debug("BI_ABSTATE_CHANGE_LIST");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_REGENMOVER_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_REGENMOVER_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcBriefInfoRegenMoverCmd();
            cmd.Read(packet);

            if (cmd.Handle != 0)
            {
                var mover = new Mover(cmd.Handle, cmd.ID);
                if (connection.Account.ActiveCharacter.VisibleObjects.Add(mover))
                {
                    Logger.Debug($"BI_REGENMOVER: Added {mover}");
                }
                else
                {
                    Logger.Error($"BI_REGENMOVER: Failed to add mover - Handle: {mover.Handle}");
                }
            }
            else
            {
                Logger.Error("Failed to read REGENMOVER packet!");
            }
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
