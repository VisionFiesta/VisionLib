using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Packet;
using VisionLib.Core.Stream;

namespace VisionLib.Core.Struct.User
{
    public class NcUserLoginWorldReq : NetPacketStruct
    {
        public const int UsernameLen = 256;
        public const int ConnectionHashLen = 64;

        public string Username { get; private set; }
        public byte[] ConnectionHash { get; private set; }

        public NcUserLoginWorldReq(string username, byte[] connectionHash)
        {
            Username = username;
            ConnectionHash = connectionHash;
        }

        public override FiestaNetPacket ToPacket()
        {
            var pkt = new FiestaNetPacket(FiestaNetCommand.NC_USER_LOGINWORLD_REQ);
            Write(pkt.Writer);
            return pkt;
        }
        public override int GetSize()
        {
            return UsernameLen + ConnectionHashLen;
        }

        public override void Read(ReaderStream reader)
        {
            Username = reader.ReadString(UsernameLen);
            ConnectionHash = reader.ReadBytes(ConnectionHashLen);
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(Username, UsernameLen);
            writer.Write(ConnectionHash, ConnectionHashLen);
        }
    }
}
