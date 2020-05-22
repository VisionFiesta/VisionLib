using System.IO;
using System.Text;

namespace Vision.Core.Extensions
{
    public static class BinaryReaderExtensions
    {
        public static string ReadString(this BinaryReader reader, int length)
        {
            var ret = string.Empty;
            var offset = 0;
            var buffer = reader.ReadBytes(length);

            while (offset < length && buffer[offset] != 0x00)
            {
                offset++;
            }

            if (length > 0)
            {
                ret = Encoding.UTF8.GetString(buffer, 0, offset);
            }

            return ret;
        }
    }
}