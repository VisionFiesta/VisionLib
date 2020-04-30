namespace Vision.Core.Networking.Packet
{
    public delegate void FiestaNetPacketHandlerDelegate<in T>(FiestaNetPacket packet, T connection) where T : FiestaNetConnection;
}
