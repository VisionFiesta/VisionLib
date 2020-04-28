using VisionLib.Common.Networking;
using VisionLib.Core.Stream;
using VisionLib.Core.Struct.Common;

namespace VisionLib.Core.Struct.Char
{
    public class NcCharChangeParamChangeCmd : NetPacketStruct
    {
        public byte Count;
        public CharParamChangeCmd[] Changes;

        public override int GetSize() => 1 + Count * 5;

        public override void Read(ReaderStream reader)
        {
            Count = reader.ReadByte();

            Changes = new CharParamChangeCmd[Count];
            for (var i = 0; i < Changes.Length; i++)
            {
                Changes[i] = new CharParamChangeCmd();
                Changes[i].Read(reader);
            }
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(Count);
            foreach (var change in Changes)
            {
                change.Write(writer);
            }
        }

        public override FiestaNetCommand GetCommand() => FiestaNetCommand.NC_CHAR_CHANGEPARAMCHANGE_CMD;
    }
}
