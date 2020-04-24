using System;
using System.Globalization;
using VisionLib.Common.Networking.Packet;

namespace VisionLib.Common.Networking.Structs.User
{
    public class STRUCT_NC_USER_CLIENT_VERSION_CHECK_REQ : FiestaNetPacketStruct
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

        private DateTime ParseVersionDate(string versionDate)
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
            var pkt = new FiestaNetPacket(FiestaNetCommand.NC_USER_CLIENT_VERSION_CHECK_REQ);
            WriteToPacket(pkt);
            return pkt;
        }

        public override void WriteToPacket(FiestaNetPacket pkt)
        {
            if (pkt == null) return;
            pkt.Write(ClientBinMD5, ClientBinMD5Len);
            pkt.Write(ExtraData, ExtraDataLen);

            // unused for now
            //Write(data.VersionDate, STRUCT_NC_USER_CLIENT_VERSION_CHECK_REQ.VersionDateLen);
        }
    }
}
