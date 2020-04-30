using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;
using Vision.Game.Structs.Common;

namespace Vision.Game.Structs.Char
{
    public class NcCharClientCharTitleCmd : NetPacketStruct
    {
        public CharTitleInfo CurrentTitle;
        public ushort CurrentTitleMobID;
        public ushort TitleCount;
        public CharTitleInfo[] TitleArray;

        public override int GetSize()
        {
            return 6 + TitleCount * 2;
        }

        public override void Read(ReaderStream reader)
        {
            CurrentTitle = new CharTitleInfo();
            CurrentTitle.Read(reader);

            CurrentTitleMobID = reader.ReadUInt16();
            TitleCount = reader.ReadUInt16();

            TitleArray = new CharTitleInfo[TitleCount];
            for (var i = 0; i < TitleArray.Length; i++)
            {
                TitleArray[i] = new CharTitleInfo();
                TitleArray[i].Read(reader);
            }
        }

        public override void Write(WriterStream writer)
        {
            CurrentTitle.Write(writer);

            writer.Write(CurrentTitleMobID);
            writer.Write(TitleCount);

            foreach (var title in TitleArray)
            {
                title.Write(writer);
            }
        }

        public override FiestaNetCommand GetCommand()
        {
            return FiestaNetCommand.NC_CHAR_CLIENT_CHARTITLE_CMD;
        }
    }
}
