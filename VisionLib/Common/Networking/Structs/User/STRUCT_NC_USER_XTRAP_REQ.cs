using VisionLib.Common.Networking.Protocols.User;

namespace VisionLib.Common.Networking.Structs.User
{
    public class STRUCT_NC_USER_XTRAP_REQ : FiestaNetStruct
    {
        public readonly byte XTrapHashLength;
        public readonly string XTrapVersionHash;

        public STRUCT_NC_USER_XTRAP_REQ(byte xTrapHashLength, string xTrapVersionHash)
        {
            XTrapHashLength = xTrapHashLength;
            XTrapVersionHash = xTrapVersionHash;
        }

        public STRUCT_NC_USER_XTRAP_REQ(FiestaNetPacket packet)
        {
            XTrapHashLength = packet.ReadByte();
            XTrapVersionHash = packet.ReadString(XTrapHashLength);
        }

        public override FiestaNetPacket ToPacket()
        {
            return new PROTO_NC_USER_XTRAP_REQ(this);
        }
    }
}
