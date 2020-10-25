using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;
using Vision.Game.Enums;

namespace Vision.Game.Structs.Announce
{
    public class NcAnnounceW2CCmd : NetPacketStruct
    {
        public AnnounceType AnnounceType;
        public byte MessageLength;
        public string Message;

        public override int GetSize() => 2 + MessageLength;

        public override void Read(ReaderStream reader)
        {
            AnnounceType = (AnnounceType)reader.ReadByte();
            MessageLength = reader.ReadByte();
            Message = reader.ReadString(MessageLength);
        }

        public override void Write(WriterStream writer)
        {
            writer.Write((byte)AnnounceType);
            writer.Write(MessageLength);
            writer.Write(Message, MessageLength);
        }

        public override NetCommand GetCommand() => NetCommand.NC_ANNOUNCE_W2C_CMD;

        public override bool HasMaximumSize() => false;
    }
}
