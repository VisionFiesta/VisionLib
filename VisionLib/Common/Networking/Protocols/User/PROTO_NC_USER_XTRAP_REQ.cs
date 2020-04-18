using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Structs.User;

namespace VisionLib.Common.Network.Protocols.User
{
    public class PROTO_NC_USER_XTRAP_REQ : FiestaNetPacket
    {
        public PROTO_NC_USER_XTRAP_REQ(STRUCT_NC_USER_XTRAP_REQ data) : base(FiestaNetCommand.NC_USER_XTRAP_REQ)
        {
            Write(data.XTrapHashLength);
            Write(data.XTrapVersionHash, data.XTrapHashLength);
        }
    }
}
