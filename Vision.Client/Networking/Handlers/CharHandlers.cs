﻿using Vision.Client.Enums;
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

        [NetPacketHandler(NetCommand.NC_CHAR_LOGIN_ACK, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_CHAR_LOGIN_ACK(NetPacket packet, NetClientConnection connection)
        {
            var ack = new NcCharLoginAck();
            ack.Read(packet);

            connection.GameClient.WorldService.SetInitialZoneEndpoint(ack.ZoneEndPoint);
            connection.GameClient.WorldService.SetStatus(ClientWorldServiceStatus.CWSS_CHARLOGGEDIN);
        }

        #endregion

        #region From Zone

        [NetPacketHandler(NetCommand.NC_CHAR_CLIENT_BASE_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_CHAR_CLIENT_BASE_CMD(NetPacket packet, NetClientConnection connection)
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

        [NetPacketHandler(NetCommand.NC_CHAR_CLIENT_SHAPE_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_CHAR_CLIENT_SHAPE_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcCharClientShapeCmd();
            cmd.Read(packet);
            connection.Account.ActiveCharacter.SetShape(cmd.Shape);
            connection.GameClient.ZoneService.UpdateCharData(CharClientDataType.CCDT_SHAPE);
        }

        [NetPacketHandler(NetCommand.NC_CHAR_CLIENT_QUEST_DOING_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_CHAR_CLIENT_QUEST_DOING_CMD(NetPacket packet, NetClientConnection connection)
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

        [NetPacketHandler(NetCommand.NC_CHAR_CLIENT_QUEST_DONE_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_CHAR_CLIENT_QUEST_DONE_CMD(NetPacket packet, NetClientConnection connection)
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

        [NetPacketHandler(NetCommand.NC_CHAR_CLIENT_QUEST_READ_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_CHAR_CLIENT_QUEST_READ_CMD(NetPacket packet, NetClientConnection connection)
        {
            // TODO: struct
            connection.GameClient.ZoneService.UpdateCharData(CharClientDataType.CCDT_QUESTREAD);
        }

        [NetPacketHandler(NetCommand.NC_CHAR_CLIENT_QUEST_REPEAT_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_CHAR_CLIENT_QUEST_REPEAT_CMD(NetPacket packet, NetClientConnection connection)
        {
            // TODO: struct
            connection.GameClient.ZoneService.UpdateCharData(CharClientDataType.CCDT_QUESTREPEAT);
        }

        [NetPacketHandler(NetCommand.NC_CHAR_CLIENT_SKILL_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_CHAR_CLIENT_SKILL_CMD(NetPacket packet, NetClientConnection connection)
        {
            // TODO: struct
            connection.GameClient.ZoneService.UpdateCharData(CharClientDataType.CCDT_SKILL);
        }

        [NetPacketHandler(NetCommand.NC_CHAR_CLIENT_PASSIVE_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_CHAR_CLIENT_PASSIVE_CMD(NetPacket packet, NetClientConnection connection)
        {
            // TODO: struct
            connection.GameClient.ZoneService.UpdateCharData(CharClientDataType.CCDT_PASSIVE);
        }

        [NetPacketHandler(NetCommand.NC_CHAR_CLIENT_ITEM_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_CHAR_CLIENT_ITEM_CMD(NetPacket packet, NetClientConnection connection)
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

        [NetPacketHandler(NetCommand.NC_CHAR_CLIENT_CHARTITLE_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_CHAR_CLIENT_CHARTITLE_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcCharClientCharTitleCmd();
            cmd.Read(packet);
            connection.Account.ActiveCharacter.UpdateTitles(cmd.CurrentTitle, cmd.TitleArray);
            connection.GameClient.ZoneService.UpdateCharData(CharClientDataType.CCDT_TITLE);
        }

        [NetPacketHandler(NetCommand.NC_CHAR_CLIENT_CHARGEDBUFF_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_CHAR_CLIENT_CHARGEDBUFF_CMD(NetPacket packet, NetClientConnection connection)
        {
            // TODO: struct
            connection.GameClient.ZoneService.UpdateCharData(CharClientDataType.CCDT_CHARGEDBUFF);
        }

        [NetPacketHandler(NetCommand.NC_CHAR_CLIENT_GAME_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_CHAR_CLIENT_GAME_CMD(NetPacket packet, NetClientConnection connection)
        {
            // TODO: struct
            connection.GameClient.ZoneService.UpdateCharData(CharClientDataType.CCDT_GAME);
        }

        [NetPacketHandler(NetCommand.NC_CHAR_CLIENT_COININFO_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_CHAR_CLIENT_COININFO_CMD(NetPacket packet, NetClientConnection connection)
        {
            // TODO: struct
            connection.GameClient.ZoneService.UpdateCharData(CharClientDataType.CCDT_COININFO);
        }

        #endregion
    }
}