using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.User
{
    public class NcUserClientVersionCheckReq : NetPacketStruct
    {
        private const int ClientBinMd5Len = 32;
        private const int ExtraDataLen = 32;

        private string ClientBinMd5 { get; set; }
        public byte[] ExtraData { get; private set; }
        private int _allDataLen;

        public NcUserClientVersionCheckReq(byte[] allData)
        {
            ExtraData = allData;
            _allDataLen = 64;
        }

        public NcUserClientVersionCheckReq(string clientBinMd5, byte[] allExtraData)
        {
            ClientBinMd5 = clientBinMd5;
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

        public override int GetSize()
        {
            return 64;
        }

        public override void Read(ReaderStream reader)
        {
            ClientBinMd5 = reader.ReadString(ClientBinMd5Len);
            ExtraData = reader.ReadBytes(ExtraDataLen);
            _allDataLen = 0;
        }

        public override void Write(WriterStream writer)
        {
            if (_allDataLen != 0)
            {
                writer.Write(ExtraData, _allDataLen);
            }
            else
            {
                writer.Write(ClientBinMd5, ClientBinMd5Len);
                writer.Write(ExtraData, ExtraDataLen);
            }
        }

        protected override NetCommand GetCommand()
        {
            return NetCommand.NC_USER_CLIENT_VERSION_CHECK_REQ;
        }
    }
}
