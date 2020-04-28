using VisionLib.Common.Networking;
using VisionLib.Core.Stream;
using VisionLib.Core.Struct.Common;

namespace VisionLib.Core.Struct.CharOption
{
    public class NcCharOptionImproveGetGameOptionDataCmd : NetPacketStruct
    {
        public ushort Count; // +2
        public GameOptionData[] Data; // +(Count * 3)

        public NcCharOptionImproveGetGameOptionDataCmd(params GameOptionData[] data)
        {
            Count = (ushort) data.Length;
            Data = data;
        }

        public override int GetSize()
        {
            return sizeof(ushort) + Count * 3;
        }

        public override void Read(ReaderStream reader)
        {
            Count = reader.ReadUInt16();
            Data = new GameOptionData[Count];
            for (var i = 0; i < Data.Length; i++)
            {
                Data[i] = new GameOptionData();
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
            return FiestaNetCommand.NC_CHAR_OPTION_IMPROVE_GET_GAMEOPTION_CMD;
        }
    }
}
