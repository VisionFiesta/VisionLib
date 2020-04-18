namespace VisionLib.Common.Networking.Protocols.User
{
    public class PROTO_NC_USER_WORLDSELECT_REQ : FiestaNetPacket
    {
        public PROTO_NC_USER_WORLDSELECT_REQ(byte worldID) : base(FiestaNetCommand.NC_USER_WORLDSELECT_REQ)
        {
            Write(worldID);
        }
    }
}
