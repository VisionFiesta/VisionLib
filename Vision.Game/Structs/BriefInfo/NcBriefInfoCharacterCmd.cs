using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.BriefInfo
{
    public class NcBriefInfoCharacterCmd : NetPacketStruct
    {
        public byte CharacterCount;
        public NcBriefInfoLoginCharacterCmd[] Characters;

        public override int GetSize() => 1 + 248 * CharacterCount;

        public override void Read(ReaderStream reader)
        {
            CharacterCount = reader.ReadByte();

            Characters = new NcBriefInfoLoginCharacterCmd[CharacterCount];
            for (var i = 0; i < CharacterCount; i++)
            {
                Characters[i] = new NcBriefInfoLoginCharacterCmd();
                Characters[i].Read(reader);
            }

        }

        public override void Write(WriterStream writer)
        {
            writer.Write(CharacterCount);
            foreach (var chr in Characters)
            {
                chr.Write(writer);
            }
        }

        public override FiestaNetCommand GetCommand() => FiestaNetCommand.NC_BRIEFINFO_CHARACTER_CMD;
    }
}
