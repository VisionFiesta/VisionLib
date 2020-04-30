using Vision.Core.Common;
using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.User
{
    public class NcUserGERLoginReq : NetPacketStruct
    {
        public string Username;
        public string Password;
        public string SpawnApps;

        public NcUserGERLoginReq(string username, string password)
        {
            Username = username;
            Password = password;
            SpawnApps = "xD";
        }

        public override int GetSize()
        {
            return 54;
        }

        public override void Read(ReaderStream reader)
        {
            Username = reader.ReadString(NameN.Name18ByteLen);
            Password = reader.ReadString(NameN.Name4Len);
            SpawnApps = reader.ReadString(NameN.Name5Len);
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(Username, NameN.Name18ByteLen);
            writer.Write(Password, NameN.Name4Len);
            writer.Write(SpawnApps, NameN.Name5Len);
        }

        public override FiestaNetCommand GetCommand()
        {
            return FiestaNetCommand.NC_USER_GER_LOGIN_REQ;
        }
    }
}
