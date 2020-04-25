using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Packet;
using VisionLib.Core.Stream;

namespace VisionLib.Core.Struct.Announce
{
    public class NcAnnounceW2CCmd : NetPacketStruct
    {
        public ANNOUNCE_TYPE AnnounceType;
        public byte MessageLength;
        public string Message;

        public override FiestaNetPacket ToPacket()
        {
            var pkt = new FiestaNetPacket(FiestaNetCommand.NC_ANNOUNCE_W2C_CMD);
            Write(pkt.Writer);
            return pkt;
        }

        public override int GetSize()
        {
            return sizeof(byte) * 2 + MessageLength;
        }

        public override void Read(ReaderStream reader)
        {
            AnnounceType = (ANNOUNCE_TYPE)reader.ReadByte();
            MessageLength = reader.ReadByte();
            Message = reader.ReadString(MessageLength);
        }

        public override void Write(WriterStream writer)
        {
            writer.Write((byte)AnnounceType);
            writer.Write(MessageLength);
            writer.Write(Message, MessageLength);
        }
    }
    
    public enum ANNOUNCE_TYPE : byte
    {
        AT_LV20 = 4,
        AT_PROMOTE = 5,
        AT_TITLEACQUIRE = 6,
        AT_ROAR = 11,
    }
}
