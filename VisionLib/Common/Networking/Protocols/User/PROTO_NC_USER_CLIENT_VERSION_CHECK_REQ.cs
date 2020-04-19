using VisionLib.Common.Networking.Structs.User;

namespace VisionLib.Common.Networking.Protocols.User
{
    public class PROTO_NC_USER_CLIENT_VERSION_CHECK_REQ : FiestaNetPacket
    {
        public PROTO_NC_USER_CLIENT_VERSION_CHECK_REQ(STRUCT_NC_USER_CLIENT_VERSION_CHECK_REQ data) : base(FiestaNetCommand.NC_USER_CLIENT_VERSION_CHECK_REQ)
        {
            Write(data.ClientBinMD5, STRUCT_NC_USER_CLIENT_VERSION_CHECK_REQ.ClientBinMD5Len);
            //Write(data.VersionDate, STRUCT_NC_USER_CLIENT_VERSION_CHECK_REQ.VersionDateLen);
            Write(data.ExtraData, STRUCT_NC_USER_CLIENT_VERSION_CHECK_REQ.ExtraDataLen);
        }
    }
}
