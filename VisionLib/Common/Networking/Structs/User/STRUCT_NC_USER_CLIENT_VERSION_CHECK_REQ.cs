using System;
using System.Globalization;
using System.Linq;
using System.Text;
using VisionLib.Common.Network.Protocols.User;

namespace VisionLib.Common.Networking.Structs.User
{
    public class STRUCT_NC_USER_CLIENT_VERSION_CHECK_REQ : FiestaNetStruct
    {
        internal const int ClientBinMD5Len = 32;
        //internal const int VersionDateLen = 14;
        internal const int ExtraDataLen = 32; // 18

        public readonly string ClientBinMD5;
        public readonly byte[] ExtraData;

        public STRUCT_NC_USER_CLIENT_VERSION_CHECK_REQ(string clientBinMd5, byte[] allExtraData)
        {
            ClientBinMD5 = clientBinMd5;
            ExtraData = allExtraData;
        }

        //public STRUCT_NC_USER_CLIENT_VERSION_CHECK_REQ(string clientBinMd5, string versionDate, byte[] extraData)
        //{
        //    ClientBinMD5 = clientBinMd5;
        //    ExtraData = extraData;
        //}

        public STRUCT_NC_USER_CLIENT_VERSION_CHECK_REQ(FiestaNetPacket packet)
        {
            if (packet.Command != FiestaNetCommand.NC_USER_CLIENT_VERSION_CHECK_REQ)
            {
                throw new InvalidOperationException("Wrong command for struct!");
            }

            ClientBinMD5 = packet.ReadString(ClientBinMD5Len);
            ExtraData = packet.ReadBytes(ExtraDataLen);

            //var trueVersionDate = parseVersionDate(versionDate);
        }

        private DateTime parseVersionDate(string versionDate)
        {
            if (DateTime.TryParseExact(versionDate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var trueVersionDate))
            {
                return trueVersionDate;
            }
            else
            {
                throw new Exception("Bad versiondate!");
            }
        }

        public override FiestaNetPacket ToPacket()
        {
            return new PROTO_NC_USER_CLIENT_VERSION_CHECK_REQ(this);
        }
    }
}
