using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.BriefInfo
{
    public class NcBriefInfoInformCmd : NetPacketStruct
    {
        public ushort MyHandle;
        public NetCommand NetCommand;
        public ushort OtherHandle;

        public override int GetSize() => 6;

        public override void Read(ReaderStream reader)
        {
            MyHandle = reader.ReadUInt16();
            NetCommand = (NetCommand) reader.ReadUInt16();
            OtherHandle = reader.ReadUInt16();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(MyHandle);
            writer.Write((ushort)NetCommand);
            writer.Write(OtherHandle);
        }

        public override NetCommand GetCommand() => NetCommand.NC_BRIEFINFO_INFORM_CMD;

        public override string ToString() => $"{nameof(MyHandle)}: {MyHandle}, {nameof(NetCommand)}: {NetCommand}, {nameof(OtherHandle)}: {OtherHandle}";
    }
}
