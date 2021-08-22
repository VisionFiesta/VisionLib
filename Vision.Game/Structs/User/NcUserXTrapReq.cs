using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.User
{
    public class NcUserXTrapReq : NetPacketStruct
    {
        public byte XTrapHashLength { get; private set; }
        public byte[] XTrapVersionHash { get; private set; }

        public NcUserXTrapReq(byte[] xTrapVersionHash)
        {
            XTrapHashLength = (byte) xTrapVersionHash.Length;
            XTrapVersionHash = xTrapVersionHash;
        }

        public override int GetSize() => sizeof(byte) + XTrapHashLength;

        public override void Read(ReaderStream reader)
        {
            XTrapHashLength = reader.ReadByte();
            XTrapVersionHash = reader.ReadBytes(XTrapHashLength);
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(XTrapHashLength);
            writer.Write(XTrapVersionHash, XTrapHashLength);
        }

        protected override NetCommand GetCommand() => NetCommand.NC_USER_XTRAP_REQ;
    }
}
