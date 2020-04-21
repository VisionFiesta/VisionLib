namespace VisionLib.Common.Networking.Packet
{
    public abstract class FiestaNetPacketStruct : VisionObject
    {
        public abstract FiestaNetPacket ToPacket();
        public abstract void WriteToPacket(FiestaNetPacket pkt);
    }
}
