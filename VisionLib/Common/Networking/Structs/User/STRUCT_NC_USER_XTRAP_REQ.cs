using System;
using VisionLib.Common.Network.Protocols.User;

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
            if (packet.Command != FiestaNetCommand.NC_USER_XTRAP_REQ)
            {
                throw new InvalidOperationException("Wrong command for struct!");
            }

            XTrapHashLength = packet.ReadByte();
            XTrapVersionHash = packet.ReadString(XTrapHashLength);
        }

        public override FiestaNetPacket ToPacket()
        {
            return new PROTO_NC_USER_XTRAP_REQ(this);
        }
    }
}
