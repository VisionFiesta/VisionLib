using VisionLib.Common.Networking;
using VisionLib.Core.Stream;
using VisionLib.Core.Struct.Common;

namespace VisionLib.Core.Struct.Char
{
    public class NcCharClientQuestDoingCmd : NetPacketStruct
    {
        public uint CharNo;
        public bool NeedClear; // byte
        public byte DoingQuestCount;

        public PlayerQuestInfo[] QuestDoingArray;

        public override int GetSize() => 6 + DoingQuestCount * 32;

        public override void Read(ReaderStream reader)
        {
            CharNo = reader.ReadUInt32();
            NeedClear = reader.ReadBoolean();
            DoingQuestCount = reader.ReadByte();

            QuestDoingArray = new PlayerQuestInfo[DoingQuestCount];
            for (var i = 0; i < QuestDoingArray.Length; i++)
            {
                QuestDoingArray[i] = new PlayerQuestInfo();
                QuestDoingArray[i].Read(reader);
            }
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(CharNo);
            writer.Write(NeedClear);
        }

        public override FiestaNetCommand GetCommand() => FiestaNetCommand.NC_CHAR_CLIENT_QUEST_DOING_CMD;
    }
}
