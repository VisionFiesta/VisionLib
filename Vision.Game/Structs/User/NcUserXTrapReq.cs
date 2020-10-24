using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.User
{
    public class NcUserXTrapReq : NetPacketStruct
    {
        public byte XTrapHashLength { get; private set; }
        public string XTrapVersionHash { get; private set; }

        public NcUserXTrapReq(string xTrapVersionHash)
        {
            XTrapHashLength = (byte) xTrapVersionHash.Length;
            XTrapVersionHash = xTrapVersionHash;
        }

        public NcUserXTrapReq(byte xTrapHashLength, string xTrapVersionHash)
        {
            XTrapHashLength = xTrapHashLength;
            XTrapVersionHash = xTrapVersionHash;
        }

        public override int GetSize()
        {
            return sizeof(byte) + XTrapHashLength;
        }

        public override void Read(ReaderStream reader)
        {
            XTrapHashLength = reader.ReadByte();
            XTrapVersionHash = reader.ReadString(XTrapHashLength);
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(XTrapHashLength);
            writer.Write(XTrapVersionHash, XTrapHashLength);
        }

        public override NetCommand GetCommand()
        {
            return NetCommand.NC_USER_XTRAP_REQ;
        }
    }
}
