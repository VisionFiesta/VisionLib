using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Packet;
using VisionLib.Core.Stream;

namespace VisionLib.Core.Struct.User
{
    public class NcUserWorldSelectReq : NetPacketStruct
    {
        public byte WorldID { get; private set; }

        public NcUserWorldSelectReq(byte worldID)
        {
            WorldID = worldID;
        }

        public override FiestaNetPacket ToPacket()
        {
            var pkt = new FiestaNetPacket(FiestaNetCommand.NC_USER_WORLDSELECT_REQ);
            Write(pkt.Writer);
            return pkt;
        }
        public override int GetSize()
        {
            return sizeof(byte);
        }

        public override void Read(ReaderStream reader)
        {
            WorldID = reader.ReadByte();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(WorldID);
        }
    }
}
