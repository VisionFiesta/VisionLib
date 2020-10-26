using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.Map
{
    public class NcMapLoginFailAck : NetPacketStruct
    {
        public ushort Error;
        public byte WrongDataFileIndex;

        public override int GetSize() => 3;

        public override void Read(ReaderStream reader)
        {
            Error = reader.ReadUInt16();
            WrongDataFileIndex = reader.ReadByte();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(Error);
            writer.Write(WrongDataFileIndex);
        }

        public override NetCommand GetCommand() => NetCommand.NC_MAP_LOGINFAIL_ACK;

        public override string ToString() => $"{nameof(Error)}: {Error}, {nameof(WrongDataFileIndex)}: {WrongDataFileIndex}";
    }
}
