using VisionLib.Common.Networking.Packet;

namespace VisionLib.Common.Networking.Structs.Char
{
    class STRUCT_NC_CHAR_LOGIN_REQ : FiestaNetPacketStruct
    {
        public readonly byte CharSlot;

        public STRUCT_NC_CHAR_LOGIN_REQ(byte charSlot)
        {
            CharSlot = charSlot;
        }

        public override FiestaNetPacket ToPacket()
        {
            var pkt = new FiestaNetPacket(FiestaNetCommand.NC_CHAR_LOGIN_REQ);
            WriteToPacket(pkt);
            return pkt;
        }

        public override void WriteToPacket(FiestaNetPacket pkt)
        {
            pkt.Write(CharSlot);
        }
    }
}
