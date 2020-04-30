using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.Common
{
    public class PlayerQuestInfo : AbstractStruct
    {
        public uint ID;
        public byte Status;
        public PlayerQuestData Data;

        public override int GetSize()
        {
            return 32;
        }

        public override void Read(ReaderStream reader)
        {
            ID = reader.ReadUInt32();
            Status = reader.ReadByte();

            Data = new PlayerQuestData();
            Data.Read(reader);
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(ID);
            writer.Write(Status);

            Data.Write(writer);
        }
    }
}
