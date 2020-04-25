using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Packet;
using VisionLib.Core.Stream;

namespace VisionLib.Core.Struct.User
{
    public class NcUserClientVersionCheckReq : NetPacketStruct
    {
        internal const int ClientBinMD5Len = 32;
        internal const int ExtraDataLen = 32;

        public string ClientBinMD5 { get; private set; }
        public byte[] ExtraData { get; private set; }

        public NcUserClientVersionCheckReq(string clientBinMd5, byte[] allExtraData)
        {
            ClientBinMD5 = clientBinMd5;
            ExtraData = allExtraData;
        }

        // private DateTime ParseVersionDate(string versionDate)
        // {
        //     if (DateTime.TryParseExact(versionDate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var trueVersionDate))
        //     {
        //         return trueVersionDate;
        //     }
        //     else
        //     {
        //         throw new Exception("Bad versiondate!");
        //     }
        // }

        public override FiestaNetPacket ToPacket()
        {
            var pkt = new FiestaNetPacket(FiestaNetCommand.NC_USER_CLIENT_VERSION_CHECK_REQ);
            Write(pkt.Writer);
            return pkt;
        }

        public override int GetSize()
        {
            return 64;
        }

        public override void Read(ReaderStream reader)
        {
            ClientBinMD5 = reader.ReadString(ClientBinMD5Len);
            ExtraData = reader.ReadBytes(ExtraDataLen);
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(ClientBinMD5, ClientBinMD5Len);
            writer.Write(ExtraData, ExtraDataLen);
        }
    }
}
