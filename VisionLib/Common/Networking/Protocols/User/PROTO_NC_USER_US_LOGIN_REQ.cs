using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Structs.User;

namespace VisionLib.Common.Network.Protocols.User
{
    public class PROTO_NC_USER_US_LOGIN_REQ : FiestaNetPacket
    {
        public PROTO_NC_USER_US_LOGIN_REQ(STRUCT_NC_USER_US_LOGIN_REQ data) : base(FiestaNetCommand.NC_USER_US_LOGIN_REQ)
        {
            Write(data.Username, STRUCT_NC_USER_US_LOGIN_REQ.UsernameLen);
            Write(data.PasswordMD5, STRUCT_NC_USER_US_LOGIN_REQ.PasswordMD5Len);
            Write(data.SpawnApp, STRUCT_NC_USER_US_LOGIN_REQ.StartupAppLen);
        }
    }
}
