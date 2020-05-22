using Vision.Core.Streams;

namespace Vision.Core.Structs
{
    public abstract class AbstractStruct
    {
        public abstract int GetSize();

        public void Read(byte[] buffer)
        {
            Read(buffer, 0);
        }

        public void Read(byte[] buffer, int offset)
        {
            var reader = new ReaderStream(buffer);
            reader.SetOffset(offset);

            Read(reader);
        }

        public abstract void Read(ReaderStream reader);

        public byte[] Write()
        {
            var writer = new WriterStream(GetSize());

            Write(writer);

            return writer.GetBuffer();
        }

        public abstract void Write(WriterStream writer);
    }
}