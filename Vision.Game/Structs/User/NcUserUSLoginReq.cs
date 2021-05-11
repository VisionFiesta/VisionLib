using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;
using Vision.Core.Utils;

namespace Vision.Game.Structs.User
{
    public class NcUserUSLoginReq : NetPacketStruct
    {
        internal const int BeginningFillerLen = 32;
        internal const int UsernameLen = 260;
        internal const int PasswordMD5Len = 32;
        internal const int SecondFillerLen = 5;
        internal const int StartupAppLen = 20;

        public string Username { get; private set; }
        public string PasswordMD5 { get; private set; }
        public string SpawnApp { get; private set; } // NA default: "Original"

        public NcUserUSLoginReq(string username, string password, string spawnApp = "Original", bool isPassPlaintext = true)
        {
            Username = username;
            PasswordMD5 = isPassPlaintext ? Md5Utils.CalcMd5(password).ToLower() : password;
            SpawnApp = spawnApp;
        }

        public override int GetSize()
        {
            return BeginningFillerLen + UsernameLen + PasswordMD5Len + SecondFillerLen + StartupAppLen;
        }

        public override void Read(ReaderStream reader)
        {
            reader.ReadBytes(BeginningFillerLen); // filler
            Username = reader.ReadString(UsernameLen);
            PasswordMD5 = reader.ReadString(PasswordMD5Len);
            reader.ReadBytes(SecondFillerLen); // filler
            SpawnApp = reader.ReadString(StartupAppLen);
        }

        public override void Write(WriterStream writer)
        {
            writer.Fill(BeginningFillerLen, 0);
            writer.Write(Username, UsernameLen);
            writer.Write(PasswordMD5, PasswordMD5Len);
            writer.Fill(SecondFillerLen, 0); // wacky filler
            writer.Write(SpawnApp, StartupAppLen);
        }

        public override NetCommand GetCommand()
        {
            return NetCommand.NC_USER_US_LOGIN_REQ;
        }
    }
}
