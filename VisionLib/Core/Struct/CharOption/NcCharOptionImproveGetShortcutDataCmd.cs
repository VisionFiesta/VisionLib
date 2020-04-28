using VisionLib.Common.Networking;
using VisionLib.Core.Stream;
using VisionLib.Core.Struct.Common;

namespace VisionLib.Core.Struct.CharOption
{
    public class NcCharOptionImproveGetShortcutDataCmd : NetPacketStruct
    {
        public ushort Count; // +2
        public ShortCutData[] Data; // +(Count * 7)

        public NcCharOptionImproveGetShortcutDataCmd(params ShortCutData[] data)
        {
            Count = (ushort) data.Length;
            Data = data;
        }

        public override int GetSize()
        {
            return sizeof(ushort) + Count * 7;
        }

        public override void Read(ReaderStream reader)
        {
            Count = reader.ReadUInt16();
            Data = new ShortCutData[Count];
            for (var i = 0; i < Data.Length; i++)
            {
                Data[i] = new ShortCutData();
                Data[i].Read(reader);
            }
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(Count);
            foreach (var data in Data)
            {
                data.Write(writer);
            }
        }

        public override FiestaNetCommand GetCommand()
        {
            return FiestaNetCommand.NC_CHAR_OPTION_IMPROVE_GET_SHORTCUTDATA_CMD;
        }
    }
}
