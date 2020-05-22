using System.IO;
using System.Text;

namespace Vision.Core.IO.SHN
{
    public class SHNBinaryReader : BinaryReader
    {
        private readonly Encoding _shnEncoding;

        private readonly byte[] _buffer = new byte[256];

        public SHNBinaryReader(Stream s, Encoding se) : base(s) => _shnEncoding = se;

        public string ReadString(int bytes) => bytes > 0 ? ReadString((uint)bytes) : string.Empty;

        public string ReadString(uint bytes) => PReadString(bytes).TrimEnd(new char[1]);

        private string PReadString(uint bytes)
        {
            var returnString = string.Empty;

            if (bytes > 0x100) { returnString = ReadString(bytes - 0x100); }

            Read(_buffer, 0, (int)bytes);

            return returnString + _shnEncoding.GetString(_buffer, 0, (int)bytes);
        }

        public override string ReadString()
        {
            var count = 0;

            for (var counter = ReadByte(); counter != 0; counter = ReadByte())
            {
                _buffer[count++] = counter;

                if (count >= 0x100) { break; }
            }

            var returnString = _shnEncoding.GetString(_buffer, 0, count);

            if (count == 0x100) { returnString += ReadString(); }

            return returnString;
        }
    }
}
