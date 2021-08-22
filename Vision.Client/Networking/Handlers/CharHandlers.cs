using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Vision.Client.Services;
using Vision.Core.Logging.Loggers;
using Vision.Core.Networking;
using Vision.Core.Networking.Packet;
using Vision.Game.Structs.Char;
using static Vision.Client.Services.ZoneCharacterDataManager;

namespace Vision.Client.Networking.Handlers
{
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class CharHandlers
    {
        private static readonly ClientLog Logger = new(typeof(CharHandlers));

        #region From WorldManager

        [NetPacketHandler(NetCommand.NC_CHAR_LOGIN_ACK, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_CHAR_LOGIN_ACK(NetPacket packet, NetClientConnection connection)
        {
            var ack = new NcCharLoginAck();
            ack.Read(packet);

            connection.GameClient.ClientSessionData.ActiveZoneEndPoint = ack.ZoneEndPoint;
            connection.UpdateWorldService(WorldServiceTrigger.WST_CHAR_LOGIN_OK);
        }

        [NetPacketHandler(NetCommand.NC_CHAR_LOGINFAIL_ACK, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_CHAR_LOGINFAIL_ACK(NetPacket packet, NetClientConnection connection)
        {
            var ack = new NcCharLoginFailAck();
            ack.Read(packet);

            connection.GameClient.ClientSessionData.ActiveZoneEndPoint = null;
            connection.UpdateWorldService(WorldServiceTrigger.WST_CHAR_LOGIN_FAIL);
        }

        #endregion

        #region From Zone

        [NetPacketHandler(NetCommand.NC_CHAR_CLIENT_BASE_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_CHAR_CLIENT_BASE_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcCharClientBaseCmd();
            cmd.Read(packet);

            var avatar = connection.Account.ActiveAvatar;
            if (avatar != null)
            {
                avatar.Update(cmd);
                connection.UpdateZoneService(CharClientDataType.CCDT_BASE);
            }
            else
            {
                Logger.Error("Failed to update character Base Data - WorldCharacter uninitialized.");
                connection.UpdateZoneService(ZoneServiceTrigger.ZST_LOGIN_CHARACTER_FAIL);
            } 
        }

        [NetPacketHandler(NetCommand.NC_CHAR_CLIENT_SHAPE_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_CHAR_CLIENT_SHAPE_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcCharClientShapeCmd();
            cmd.Read(packet);

            var avatar = connection.Account.ActiveAvatar;

            if (avatar != null)
            {
                avatar.Update(cmd);
                connection.UpdateZoneService(CharClientDataType.CCDT_SHAPE);
            }
            else
            {
                Logger.Error("Failed to update character Shape Data - WorldCharacter uninitialized.");
                connection.UpdateZoneService(ZoneServiceTrigger.ZST_LOGIN_CHARACTER_FAIL);
            }
        }

        [NetPacketHandler(NetCommand.NC_CHAR_CLIENT_QUEST_DOING_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_CHAR_CLIENT_QUEST_DOING_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcCharClientQuestDoingCmd();
            cmd.Read(packet);

            var avatar = connection.Account.ActiveAvatar;
            if (avatar != null)
            {
                avatar.Update(cmd);
                connection.UpdateZoneService(CharClientDataType.CCDT_QUESTDOING);
            }
            else
            {
                Logger.Error("Failed to update character QuestDoing Data - WorldCharacter uninitialized.");
                connection.UpdateZoneService(ZoneServiceTrigger.ZST_LOGIN_CHARACTER_FAIL);
            }
        }

        [NetPacketHandler(NetCommand.NC_CHAR_CLIENT_QUEST_DONE_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_CHAR_CLIENT_QUEST_DONE_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcCharClientQuestDoneCmd();
            cmd.Read(packet);

            var avatar = connection.Account.ActiveAvatar;
            if (avatar != null)
            {
                avatar.Update(cmd);
                connection.UpdateZoneService(CharClientDataType.CCDT_QUESTDONE);
            }
            else
            {
                Logger.Error("Failed to update character QuestDone Data - WorldCharacter uninitialized.");
                connection.UpdateZoneService(ZoneServiceTrigger.ZST_LOGIN_CHARACTER_FAIL);
            }
        }

        [NetPacketHandler(NetCommand.NC_CHAR_CLIENT_QUEST_READ_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_CHAR_CLIENT_QUEST_READ_CMD(NetPacket packet, NetClientConnection connection)
        {
            // TODO: struct
            connection.UpdateZoneService(CharClientDataType.CCDT_QUESTREAD);
        }

        [NetPacketHandler(NetCommand.NC_CHAR_CLIENT_QUEST_REPEAT_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_CHAR_CLIENT_QUEST_REPEAT_CMD(NetPacket packet, NetClientConnection connection)
        {
            // TODO: struct
            connection.UpdateZoneService(CharClientDataType.CCDT_QUESTREPEAT);
        }

        [NetPacketHandler(NetCommand.NC_CHAR_CLIENT_SKILL_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_CHAR_CLIENT_SKILL_CMD(NetPacket packet, NetClientConnection connection)
        {
            // TODO: struct
            connection.UpdateZoneService(CharClientDataType.CCDT_SKILL);
        }

        [NetPacketHandler(NetCommand.NC_CHAR_CLIENT_PASSIVE_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_CHAR_CLIENT_PASSIVE_CMD(NetPacket packet, NetClientConnection connection)
        {
            // TODO: struct
            connection.UpdateZoneService(CharClientDataType.CCDT_PASSIVE);
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
                    connection.UpdateZoneService(CharClientDataType.CCDT_ITEM1);
                    break;
                case 8:
                    connection.UpdateZoneService(CharClientDataType.CCDT_ITEM2);
                    break;
                case 12:
                    connection.UpdateZoneService(CharClientDataType.CCDT_ITEM3);
                    break;
                case 15:
                    connection.UpdateZoneService(CharClientDataType.CCDT_ITEM4);
                    break;
            }
        }

        [NetPacketHandler(NetCommand.NC_CHAR_CLIENT_CHARTITLE_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_CHAR_CLIENT_CHARTITLE_CMD(NetPacket packet, NetClientConnection connection)
        {
            var cmd = new NcCharClientCharTitleCmd();
            cmd.Read(packet);

            var avatar = connection.Account.ActiveAvatar;
            if (avatar != null)
            {
                avatar.Update(cmd);
                connection.UpdateZoneService(CharClientDataType.CCDT_CHARTITLE);
            }
            else
            {
                Logger.Error("Failed to update character Title Data - WorldCharacter uninitialized.");
                connection.UpdateZoneService(ZoneServiceTrigger.ZST_LOGIN_CHARACTER_FAIL);
            }
        }

        [NetPacketHandler(NetCommand.NC_CHAR_CLIENT_CHARGEDBUFF_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_CHAR_CLIENT_CHARGEDBUFF_CMD(NetPacket packet, NetClientConnection connection)
        {
            // TODO: struct
            connection.UpdateZoneService(CharClientDataType.CCDT_CHARGEDBUFF);
        }

        [NetPacketHandler(NetCommand.NC_CHAR_CLIENT_GAME_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_CHAR_CLIENT_GAME_CMD(NetPacket packet, NetClientConnection connection)
        {
            // TODO: struct
            connection.UpdateZoneService(CharClientDataType.CCDT_GAME);
        }

        [NetPacketHandler(NetCommand.NC_CHAR_CLIENT_COININFO_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_CHAR_CLIENT_COININFO_CMD(NetPacket packet, NetClientConnection connection)
        {
            // TODO: struct
            connection.UpdateZoneService(CharClientDataType.CCDT_COININFO);
        }

        #endregion
    }
}
