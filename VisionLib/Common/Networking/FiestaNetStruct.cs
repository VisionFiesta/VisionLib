namespace VisionLib.Common.Networking
{
    public abstract class FiestaNetStruct : VisionObject
    {
        public abstract FiestaNetPacket ToPacket();
    }
}
