using VisionLib.Common.Collections;
using VisionLib.Common.Extensions;
using VisionLib.Common.Game.Characters;
using VisionLib.Common.Game.Content;
using VisionLib.Common.Game.Content.GameObjects;
using VisionLib.Common.Logging;
using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Packet;
using VisionLib.Core.Struct.BriefInfo;

namespace VisionLib.Client.Networking.Handlers
{
    public static class BriefInfoHandlers
    {
        [FiestaNetPacketHandler(FiestaNetCommand.NC_BRIEFINFO_UNEQUIP_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_BRIEFINFO_UNEQUIP_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            // TODO:
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_BRIEFINFO_ABSTATE_CHANGE_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_BRIEFINFO_ABSTATE_CHANGE_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
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

        [FiestaNetPacketHandler(FiestaNetCommand.NC_BRIEFINFO_ABSTATE_CHANGE_LIST_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_BRIEFINFO_ABSTATE_CHANGE_LIST_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            // TODO: 
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_BRIEFINFO_CHARACTER_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_BRIEFINFO_CHARACTER_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
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

            connection.GetAccount().ActiveCharacter.VisibleObjects.AddRange(chrList);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_BRIEFINFO_MOB_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_BRIEFINFO_MOB_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var cmd = new NcBriefInfoMobCmd();
            cmd.Read(packet);

            var mobList = new FastList<Mob>();
            foreach (var mob in cmd.Mobs)
            {
                mobList.Add(new Mob(mob));
                ClientLog.Debug($"MOB: Added mob. MobID: {mob.MobID}");
            }

            connection.GetAccount().ActiveCharacter.VisibleObjects.AddRange(mobList);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_BRIEFINFO_REGENMOB_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_BRIEFINFO_REGENMOB_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var cmd = new NcBriefInfoRegenMobCmd();
            cmd.Read(packet);

            connection.GetAccount().ActiveCharacter.VisibleObjects.Add(new Mob(cmd));
            ClientLog.Debug($"REGENMOB: Added mob. MobID: {cmd.MobID}");
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_BRIEFINFO_REGENMOVER_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_BRIEFINFO_REGENMOVER_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            // TODO:
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_BRIEFINFO_LOGINCHARACTER_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_BRIEFINFO_LOGINCHARACTER_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var cmd = new NcBriefInfoLoginCharacterCmd();
            cmd.Read(packet);

            var chr = new Character(cmd.Handle);
            chr.Initialize(cmd);

            connection.GetAccount().ActiveCharacter.VisibleObjects.Add(chr);
            ClientLog.Debug($"LOGINCHARACTER: Added character. Name: {chr.Name}, Class: {chr.Shape.Class}");
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_BRIEFINFO_MOVER_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_BRIEFINFO_MOVER_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            // TODO:
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_BRIEFINFO_BRIEFINFODELETE_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_BRIEFINFO_BRIEFINFODELETE_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var handle = packet.Reader.ReadUInt16();
            var go = GameObject.Objects.First(o => o.Handle == handle);
            if (go == null)
            {
                ClientLog.Error($"Missing GameObject for delete! Handle: {handle}");
                return;
            }

            var result = connection.GetAccount().ActiveCharacter.VisibleObjects.Remove(go);

            ClientLog.Debug(result
                ? $"BRIEFINFODELETE_CMD: Removed GameObject - Handle: {go.Handle}"
                : $"BRIEFINFODELETE_CMD: Failed to remove GameObject - Handle: {go.Handle}");
        }
    }
}
