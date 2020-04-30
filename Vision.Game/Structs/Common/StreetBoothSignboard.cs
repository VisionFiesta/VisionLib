using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.Common
{
    public class StreetBoothSignboard : AbstractStruct
    {
        public string Message;

        public override int GetSize()
        {
            return 30;
        }

        public override void Read(ReaderStream reader)
        {
            Message = reader.ReadString(30);
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(Message, 30);
        }
    }
}
