using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.User
{
    public class NcUserClientVersionCheckReq : NetPacketStruct
    {
        internal const int ClientBinMD5Len = 32;
        internal const int ExtraDataLen = 32;

        public string ClientBinMD5 { get; private set; }
        public byte[] ExtraData { get; private set; }
        private int AllDataLen = 0;

        public NcUserClientVersionCheckReq() { }

        public NcUserClientVersionCheckReq(byte[] allData)
        {
            ExtraData = allData;
            AllDataLen = 64;
        }

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

        public override int GetSize()
        {
            return 64;
        }

        public override void Read(ReaderStream reader)
        {
            ClientBinMD5 = reader.ReadString(ClientBinMD5Len);
            ExtraData = reader.ReadBytes(ExtraDataLen);
            AllDataLen = 0;
        }

        public override void Write(WriterStream writer)
        {
            if (AllDataLen != 0)
            {
                writer.Write(ExtraData, AllDataLen);
            }
            else
            {
                writer.Write(ClientBinMD5, ClientBinMD5Len);
                writer.Write(ExtraData, ExtraDataLen);
            }
        }

        public override NetCommand GetCommand()
        {
            return NetCommand.NC_USER_CLIENT_VERSION_CHECK_REQ;
        }
    }
}
