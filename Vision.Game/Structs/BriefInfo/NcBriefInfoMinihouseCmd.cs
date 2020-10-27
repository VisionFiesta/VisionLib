using System.Diagnostics.CodeAnalysis;
using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.BriefInfo
{
    public class NcBriefInfoMinihouseCmd : NetPacketStruct
    {
        public byte MinihouseNum;
        public NcBriefInfoMinihouseBuildCmd[] Minihouses;

        public override int GetSize() => 1 + NcBriefInfoMinihouseBuildCmd.Size;

        public override void Read(ReaderStream reader)
        {
            MinihouseNum = reader.ReadByte();

            Minihouses = new NcBriefInfoMinihouseBuildCmd[MinihouseNum];

            for (var i = 0; i < MinihouseNum; i++)
            {
                Minihouses[i] = new NcBriefInfoMinihouseBuildCmd();
                Minihouses[i].Read(reader);
            }
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(MinihouseNum);

            for (var i = 0; i < MinihouseNum; i++)
            {
                Minihouses[i].Write(writer);
            }
        }

        public override NetCommand GetCommand() => NetCommand.NC_BRIEFINFO_EFFECT_CMD;

        [SuppressMessage("ReSharper", "CoVariantArrayConversion")]
        public override string ToString()
        {
            var minihousesString = string.Join(", ", (object[])Minihouses);
            return $"{nameof(MinihouseNum)}: {MinihouseNum}, {nameof(Minihouses)}: {minihousesString}";
        }
    }
}