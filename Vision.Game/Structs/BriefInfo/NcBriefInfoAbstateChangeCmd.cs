using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;
using Vision.Game.Content.Data.AbnormalState;

namespace Vision.Game.Structs.BriefInfo
{
    public class NcBriefInfoAbstateChangeCmd : NetPacketStruct
    {
        public ushort Handle;
        public AbnormalStateInfo Info = new();

        public override int GetSize() => 2 + AbnormalStateInfo.Size;

        public override void Read(ReaderStream reader)
        {
            Handle = reader.ReadUInt16();

            Info.Read(reader);
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(Handle);

            Info.Write(writer);
        }

        public override NetCommand GetCommand() => NetCommand.NC_BRIEFINFO_ABSTATE_CHANGE_CMD;

        public override string ToString() => $"{nameof(Handle)}: {Handle}, {nameof(Info)}: {Info}";
    }
}
