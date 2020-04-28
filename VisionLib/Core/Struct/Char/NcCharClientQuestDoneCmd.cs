﻿using System;
using VisionLib.Common.Networking;
using VisionLib.Core.Stream;
using VisionLib.Core.Struct.Common;

namespace VisionLib.Core.Struct.Char
{
    public class NcCharClientQuestDoneCmd : NetPacketStruct
    {
        public uint CharNo;
        public ushort TotalDoneQuest;
        public ushort TotalDoneQuestSize;
        public ushort DoneQuestCount;
        public ushort Index;
        public PlayerQuestDoneInfo[] QuestDoneArray;

        public override int GetSize() => 12 + DoneQuestCount * 10;

        public override void Read(ReaderStream reader)
        {
            CharNo = reader.ReadUInt32();
            TotalDoneQuest = reader.ReadUInt16();
            TotalDoneQuestSize = reader.ReadUInt16();
            DoneQuestCount = reader.ReadUInt16();
            Index = reader.ReadUInt16();

            // TODO: correct size value?
            QuestDoneArray = new PlayerQuestDoneInfo[DoneQuestCount];
            for (var i = 0; i < QuestDoneArray.Length; i++)
            {
                QuestDoneArray[i] = new PlayerQuestDoneInfo();
                QuestDoneArray[i].Read(reader);
            }
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(CharNo);
            writer.Write(TotalDoneQuest);
            writer.Write(TotalDoneQuestSize);
            writer.Write(DoneQuestCount);
            writer.Write(Index);

            for (var i = 0; i < QuestDoneArray.Length; i++)
            {
                QuestDoneArray[i] = new PlayerQuestDoneInfo();
                QuestDoneArray[i].Write(writer);
            }
        }

        public override FiestaNetCommand GetCommand() => FiestaNetCommand.NC_CHAR_CLIENT_QUEST_DONE_CMD;
    }
}
