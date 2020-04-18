namespace VisionLib.Common.Networking
{
    public abstract class FiestaNetStruct : VisionObject
    {
        public FiestaNetStruct() { }
        public FiestaNetStruct(FiestaNetPacket packet) { }

        public abstract FiestaNetPacket ToPacket();
    }
}
