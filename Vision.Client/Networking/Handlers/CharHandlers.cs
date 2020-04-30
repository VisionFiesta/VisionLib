using Vision.Client.Enums;
using Vision.Client.Services;
using Vision.Core.Logging.Loggers;
using Vision.Core.Networking;
using Vision.Core.Networking.Packet;
using Vision.Game.Structs.Char;

namespace Vision.Client.Networking.Handlers
{
    public static class CharHandlers
    {
        #region From WorldManager

        [FiestaNetPacketHandler(FiestaNetCommand.NC_CHAR_LOGIN_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_CHAR_LOGIN_ACK(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {
            var ack = new NcCharLoginAck();
            ack.Read(packet);

            connection.GameClient.WorldService.SetInitialZoneEndpoint(ack.ZoneEndPoint);
            connection.GameClient.WorldService.SetStatus(ClientWorldServiceStatus.CWSS_CHARLOGGEDIN);
        }

        #endregion

        #region From Zone

        [FiestaNetPacketHandler(FiestaNetCommand.NC_CHAR_CLIENT_BASE_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_CHAR_CLIENT_BASE_CMD(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {
            var cmd = new NcCharClientBaseCmd();
            cmd.Read(packet);

            connection.Account.ChooseCharacter(cmd.CharNo);
            var character = connection.Account.ActiveCharacter;

            if (character == null)
            {
                ClientLog.Error("Character choose failed!");
                return;
            }

            character.Initialize(cmd);
            connection.GameClient.ZoneService.UpdateCharData(CharClientDataType.CCDT_BASE);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_CHAR_CLIENT_SHAPE_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_CHAR_CLIENT_SHAPE_CMD(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {
            var cmd = new NcCharClientShapeCmd();
            cmd.Read(packet);
            connection.Account.ActiveCharacter.SetShape(cmd.Shape);
            connection.GameClient.ZoneService.UpdateCharData(CharClientDataType.CCDT_SHAPE);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_CHAR_CLIENT_QUEST_DOING_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_CHAR_CLIENT_QUEST_DOING_CMD(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {
            var cmd = new NcCharClientQuestDoingCmd();
            cmd.Read(packet);

            var chr = connection.Account.ActiveCharacter;
            if (cmd.CharNo != chr.CharNo)
            {
                ClientLog.Warning("Got quest data for wrong CharNo!");
                return;
            }

            chr.UpdateDoingQuests(cmd.NeedClear, cmd.QuestDoingArray);
            connection.GameClient.ZoneService.UpdateCharData(CharClientDataType.CCDT_QUESTDOING);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_CHAR_CLIENT_QUEST_DONE_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_CHAR_CLIENT_QUEST_DONE_CMD(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {
            var cmd = new NcCharClientQuestDoneCmd();
            cmd.Read(packet);

            var chr = connection.Account.ActiveCharacter;
            if (cmd.CharNo != chr.CharNo)
            {
                ClientLog.Warning("Got quest data for wrong CharNo!");
                return;
            }

            chr.UpdateDoneQuests(cmd.QuestDoneArray);
            connection.GameClient.ZoneService.UpdateCharData(CharClientDataType.CCDT_QUESTDONE);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_CHAR_CLIENT_QUEST_READ_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_CHAR_CLIENT_QUEST_READ_CMD(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {
            // TODO: struct
            connection.GameClient.ZoneService.UpdateCharData(CharClientDataType.CCDT_QUESTREAD);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_CHAR_CLIENT_QUEST_REPEAT_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_CHAR_CLIENT_QUEST_REPEAT_CMD(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {
            // TODO: struct
            connection.GameClient.ZoneService.UpdateCharData(CharClientDataType.CCDT_QUESTREPEAT);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_CHAR_CLIENT_SKILL_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_CHAR_CLIENT_SKILL_CMD(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {
            // TODO: struct
            connection.GameClient.ZoneService.UpdateCharData(CharClientDataType.CCDT_SKILL);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_CHAR_CLIENT_PASSIVE_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_CHAR_CLIENT_PASSIVE_CMD(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {
            // TODO: struct
            connection.GameClient.ZoneService.UpdateCharData(CharClientDataType.CCDT_PASSIVE);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_CHAR_CLIENT_ITEM_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_CHAR_CLIENT_ITEM_CMD(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {
            // TODO: struct

            packet.Reader.ReadByte(); // packetOrder
            var nPartMark = packet.Reader.ReadByte();
            switch (nPartMark)
            {
                case 9:
                    connection.GameClient.ZoneService.UpdateCharData(CharClientDataType.CCDT_ITEM1);
                    break;
                case 8:
                    connection.GameClient.ZoneService.UpdateCharData(CharClientDataType.CCDT_ITEM2);
                    break;
                case 12:
                    connection.GameClient.ZoneService.UpdateCharData(CharClientDataType.CCDT_ITEM3);
                    break;
                case 15:
                    connection.GameClient.ZoneService.UpdateCharData(CharClientDataType.CCDT_ITEM4);
                    break;
            }
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_CHAR_CLIENT_CHARTITLE_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_CHAR_CLIENT_CHARTITLE_CMD(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {
            var cmd = new NcCharClientCharTitleCmd();
            cmd.Read(packet);
            connection.Account.ActiveCharacter.UpdateTitles(cmd.CurrentTitle, cmd.TitleArray);
            connection.GameClient.ZoneService.UpdateCharData(CharClientDataType.CCDT_TITLE);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_CHAR_CLIENT_CHARGEDBUFF_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_CHAR_CLIENT_CHARGEDBUFF_CMD(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {
            // TODO: struct
            connection.GameClient.ZoneService.UpdateCharData(CharClientDataType.CCDT_CHARGEDBUFF);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_CHAR_CLIENT_GAME_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_CHAR_CLIENT_GAME_CMD(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {
            // TODO: struct
            connection.GameClient.ZoneService.UpdateCharData(CharClientDataType.CCDT_GAME);
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_CHAR_CLIENT_COININFO_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_CHAR_CLIENT_COININFO_CMD(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {
            // TODO: struct
            connection.GameClient.ZoneService.UpdateCharData(CharClientDataType.CCDT_COININFO);
        }

        #endregion
    }
}
