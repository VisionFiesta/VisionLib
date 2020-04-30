namespace Vision.Core.Networking.Packet
{
    public delegate void NetPacketHandlerDelegate<in T>(NetPacket packet, T connection) where T : NetConnectionBase<T>;
}
