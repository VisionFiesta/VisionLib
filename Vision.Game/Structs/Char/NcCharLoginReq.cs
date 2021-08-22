using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.Char
{
    public class NcCharLoginReq : NetPacketStruct
    {
        public byte CharSlot { get; private set; }

        public NcCharLoginReq(byte charSlot)
        {
            CharSlot = charSlot;
        }

        public override int GetSize()
        {
            return sizeof(byte);
        }

        public override void Read(ReaderStream reader)
        {
            CharSlot = reader.ReadByte();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(CharSlot);
        }

        protected override NetCommand GetCommand()
        {
            return NetCommand.NC_CHAR_LOGIN_REQ;
        }
    }
}
