using Vision.Core;
using Vision.Core.Streams;
using Vision.Core.Structs;
using Vision.Game.Enums;

namespace Vision.Game.Structs.Common
{
    public class MobFlag : AbstractStruct
    {
        public const int Size = 1 + AbnormalStateBit.Size;

        public MobBriefFlag Flag;

        public AbnormalStateBit AbstateBit = new();

        public string GateToWhere = "";

        public override int GetSize() => Size;

        public override void Read(ReaderStream reader)
        {
            Flag = (MobBriefFlag)reader.ReadByte();

            switch (Flag)
            {
                case MobBriefFlag.MBF_NORMAL:
                    {
                        AbstateBit.Read(reader);
                        break;
                    }
                case MobBriefFlag.MBF_GATE:
                    {
                        GateToWhere = reader.ReadString(NameN.Name3Len);
                        reader.ReadBytes(AbnormalStateBit.Size - NameN.Name3Len); // padding
                        break;
                    }
            }
        }

        public override void Write(WriterStream writer)
        {
            writer.Write((byte)Flag);

            switch (Flag)
            {
                case MobBriefFlag.MBF_NORMAL:
                    {
                        AbstateBit.Write(writer);
                        break;
                    }
                case MobBriefFlag.MBF_GATE:
                    {
                        writer.Write(GateToWhere, NameN.Name3Len);
                        writer.Fill(AbnormalStateBit.Size - NameN.Name3Len, 0x00); // padding)
                        break;
                    }
            }
        }
    }
}
