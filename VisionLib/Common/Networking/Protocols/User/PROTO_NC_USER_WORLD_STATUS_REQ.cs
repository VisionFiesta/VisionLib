using VisionLib.Common.Networking.Packet;

namespace VisionLib.Common.Networking.Protocols.User
{
    public class PROTO_NC_USER_WORLD_STATUS_REQ : FiestaNetPacket
    {
        public PROTO_NC_USER_WORLD_STATUS_REQ() : base(FiestaNetCommand.NC_USER_WORLD_STATUS_REQ) { }
    }
}
