using VisionLib.Common.Networking.Packet;

namespace VisionLib.Common.Networking.Structs.User
{
    public class STRUCT_NC_USER_XTRAP_REQ : FiestaNetPacketStruct
    {
        public readonly byte XTrapHashLength;
        public readonly string XTrapVersionHash;

        public STRUCT_NC_USER_XTRAP_REQ(byte xTrapHashLength, string xTrapVersionHash)
        {
            XTrapHashLength = xTrapHashLength;
            XTrapVersionHash = xTrapVersionHash;
        }

        public STRUCT_NC_USER_XTRAP_REQ(FiestaNetPacket packet)
        {
            XTrapHashLength = packet.ReadByte();
            XTrapVersionHash = packet.ReadString(XTrapHashLength);
        }

        public override FiestaNetPacket ToPacket()
        {
            var pkt = new FiestaNetPacket(FiestaNetCommand.NC_USER_XTRAP_REQ);
            WriteToPacket(pkt);
            return pkt;
        }

        public override void WriteToPacket(FiestaNetPacket pkt)
        {
            pkt.Write(XTrapHashLength);
            pkt.Write(XTrapVersionHash, XTrapHashLength);
        }
    }
}
