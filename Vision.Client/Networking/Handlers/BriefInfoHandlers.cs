﻿using Vision.Core.Collections;
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
        [NetPacketHandler(NetCommand.NC_BRIEFINFO_UNEQUIP_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_UNEQUIP_CMD(NetPacket packet, NetClientConnection connection)
        {
            // TODO:
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_ABSTATE_CHANGE_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_ABSTATE_CHANGE_CMD(NetPacket packet, NetClientConnection connection)
        {
            // TODO:

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
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_ABSTATE_CHANGE_LIST_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_ABSTATE_CHANGE_LIST_CMD(NetPacket packet, NetClientConnection connection)
        {
            // TODO: 
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
                ClientLog.Debug($"CHARACTER: Added character. Name: {newChr.Name}, Class: {newChr.Shape.Class}");
            }

            connection.Account.ActiveCharacter.VisibleObjects.AddRange(chrList);
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_MOB_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_MOB_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcBriefInfoMobCmd();
            cmd.Read(packet);

            var mobList = new FastList<Mob>();
            foreach (var mob in cmd.Mobs)
            {
                mobList.Add(new Mob(mob));
                ClientLog.Debug($"MOB: Added mob. MobID: {mob.MobID}");
            }

            connection.Account.ActiveCharacter.VisibleObjects.AddRange(mobList);
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_REGENMOB_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_REGENMOB_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcBriefInfoRegenMobCmd();
            cmd.Read(packet);

            connection.Account.ActiveCharacter.VisibleObjects.Add(new Mob(cmd));
            ClientLog.Debug($"REGENMOB: Added mob. MobID: {cmd.MobID}");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_REGENMOVER_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_REGENMOVER_CMD(NetPacket packet, NetClientConnection connection)
        {
            // TODO:
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_LOGINCHARACTER_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_LOGINCHARACTER_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcBriefInfoLoginCharacterCmd();
            cmd.Read(packet);

            var chr = new Character(cmd.Handle);
            chr.Initialize(cmd);

            connection.Account.ActiveCharacter.VisibleObjects.Add(chr);
            ClientLog.Debug($"LOGINCHARACTER: Added character. Name: {chr.Name}, Class: {chr.Shape.Class}");
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_MOVER_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_MOVER_CMD(NetPacket packet, NetClientConnection connection)
        {
            // TODO:
        }

        [NetPacketHandler(NetCommand.NC_BRIEFINFO_BRIEFINFODELETE_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BRIEFINFO_BRIEFINFODELETE_CMD(NetPacket packet, NetClientConnection connection)
        {
            var handle = packet.Reader.ReadUInt16();
            var go = GameObject.Objects.First(o => o.Handle == handle);
            if (go == null)
            {
                ClientLog.Error($"Missing GameObject for delete! Handle: {handle}");
                return;
            }

            var result = connection.Account.ActiveCharacter.VisibleObjects.Remove(go);

            ClientLog.Debug(result
                ? $"BRIEFINFODELETE_CMD: Removed GameObject - Handle: {go.Handle}"
                : $"BRIEFINFODELETE_CMD: Failed to remove GameObject - Handle: {go.Handle}");
        }
    }
}