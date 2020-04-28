using VisionLib.Client.Enums;
using VisionLib.Client.Services;
using VisionLib.Common.Extensions;
using VisionLib.Common.Logging;
using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Packet;
using VisionLib.Core.Struct.Char;

namespace VisionLib.Client.Networking.Handlers
{
    public static class CharHandlers
    {
        #region From WorldManager

        [FiestaNetPacketHandler(FiestaNetCommand.NC_CHAR_LOGIN_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_CHAR_LOGIN_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var ack = new NcCharLoginAck();
            ack.Read(packet);

            connection.GetClient()?.WorldService.SetInitialZoneEndpoint(ack.ZoneEndPoint);
            connection.GetClient()?.WorldService.SetStatus(ClientWorldServiceStatus.CWSS_CHARLOGGEDIN);
        }

        #endregion

        #region From Zone

        [FiestaNetPacketHandler(FiestaNetCommand.NC_CHAR_CLIENT_BASE_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_CHAR_CLIENT_BASE_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var cmd = new NcCharClientBaseCmd();
            cmd.Read(packet);

            connection.GetAccount()?.ChooseCharacter(cmd.CharNo);
            var character = connection.GetAccount()?.ActiveCharacter;

            if (character == null)
            {
                ClientLog.Error("Character choose failed!");
                return;
            }

            character.Initialize(cmd);
            connection.GetClient().ZoneService.UpdateCharData(CharClientDataType.CCDT_BASE);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_CHAR_CLIENT_SHAPE_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_CHAR_CLIENT_SHAPE_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var cmd = new NcCharClientShapeCmd();
            cmd.Read(packet);
            connection.GetAccount().ActiveCharacter.SetShape(cmd.Shape);
            connection.GetClient().ZoneService.UpdateCharData(CharClientDataType.CCDT_SHAPE);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_CHAR_CLIENT_QUEST_DOING_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_CHAR_CLIENT_QUEST_DOING_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var cmd = new NcCharClientQuestDoingCmd();
            cmd.Read(packet);

            var chr = connection.GetAccount().ActiveCharacter;
            if (cmd.CharNo != chr.CharNo)
            {
                ClientLog.Warning("Got quest data for wrong CharNo!");
                return;
            }

            chr.UpdateDoingQuests(cmd.NeedClear, cmd.QuestDoingArray);
            connection.GetClient().ZoneService.UpdateCharData(CharClientDataType.CCDT_QUESTDOING);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_CHAR_CLIENT_QUEST_DONE_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_CHAR_CLIENT_QUEST_DONE_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var cmd = new NcCharClientQuestDoneCmd();
            cmd.Read(packet);

            var chr = connection.GetAccount().ActiveCharacter;
            if (cmd.CharNo != chr.CharNo)
            {
                ClientLog.Warning("Got quest data for wrong CharNo!");
                return;
            }

            chr.UpdateDoneQuests(cmd.QuestDoneArray);
            connection.GetClient().ZoneService.UpdateCharData(CharClientDataType.CCDT_QUESTDONE);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_CHAR_CLIENT_QUEST_READ_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_CHAR_CLIENT_QUEST_READ_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            // TODO: struct
            connection.GetClient().ZoneService.UpdateCharData(CharClientDataType.CCDT_QUESTREAD);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_CHAR_CLIENT_QUEST_REPEAT_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_CHAR_CLIENT_QUEST_REPEAT_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            // TODO: struct
            connection.GetClient().ZoneService.UpdateCharData(CharClientDataType.CCDT_QUESTREPEAT);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_CHAR_CLIENT_SKILL_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_CHAR_CLIENT_SKILL_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            // TODO: struct
            connection.GetClient().ZoneService.UpdateCharData(CharClientDataType.CCDT_SKILL);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_CHAR_CLIENT_PASSIVE_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_CHAR_CLIENT_PASSIVE_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            // TODO: struct
            connection.GetClient().ZoneService.UpdateCharData(CharClientDataType.CCDT_PASSIVE);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_CHAR_CLIENT_ITEM_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_CHAR_CLIENT_ITEM_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            // TODO: struct

            var PacketOrder = packet.Reader.ReadByte();
            var nPartMark = packet.Reader.ReadByte();
            switch (nPartMark)
            {
                case 9:
                    connection.GetClient().ZoneService.UpdateCharData(CharClientDataType.CCDT_ITEM1);
                    break;
                case 8:
                    connection.GetClient().ZoneService.UpdateCharData(CharClientDataType.CCDT_ITEM2);
                    break;
                case 12:
                    connection.GetClient().ZoneService.UpdateCharData(CharClientDataType.CCDT_ITEM3);
                    break;
                case 15:
                    connection.GetClient().ZoneService.UpdateCharData(CharClientDataType.CCDT_ITEM4);
                    break;
            }
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_CHAR_CLIENT_CHARTITLE_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_CHAR_CLIENT_CHARTITLE_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var cmd = new NcCharClientCharTitleCmd();
            cmd.Read(packet);
            connection.GetAccount().ActiveCharacter.UpdateTitles(cmd.CurrentTitle, cmd.TitleArray);
            connection.GetClient().ZoneService.UpdateCharData(CharClientDataType.CCDT_TITLE);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_CHAR_CLIENT_CHARGEDBUFF_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_CHAR_CLIENT_CHARGEDBUFF_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            // TODO: struct
            connection.GetClient().ZoneService.UpdateCharData(CharClientDataType.CCDT_CHARGEDBUFF);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_CHAR_CLIENT_GAME_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_CHAR_CLIENT_GAME_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            // TODO: struct
            connection.GetClient().ZoneService.UpdateCharData(CharClientDataType.CCDT_GAME);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_CHAR_CLIENT_COININFO_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_CHAR_CLIENT_COININFO_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            // TODO: struct
            connection.GetClient().ZoneService.UpdateCharData(CharClientDataType.CCDT_COININFO);
        }

        #endregion
    }
}
