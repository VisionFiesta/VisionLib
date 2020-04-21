using VisionLib.Common.Networking.Packet;

namespace VisionLib.Common.Networking.Structs.Char
{
    public class STRUCT_NC_CHAR_LOGINFAIL_ACK : FiestaNetPacketStruct
    {
        public readonly ushort Error;

        public STRUCT_NC_CHAR_LOGINFAIL_ACK(ushort error)
        {
            Error = error;
        }

        public override FiestaNetPacket ToPacket()
        {
            var pkt = new FiestaNetPacket(FiestaNetCommand.NC_CHAR_LOGINFAIL_ACK);
            WriteToPacket(pkt);
            return pkt;
        }

        public override void WriteToPacket(FiestaNetPacket pkt)
        {
            pkt?.Write(Error);
        }
    }
}
