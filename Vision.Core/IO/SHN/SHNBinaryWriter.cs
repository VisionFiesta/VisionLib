using System;
using System.IO;
using System.Text;

namespace Vision.Core.IO.SHN
{
    public class SHNBinaryWriter : BinaryWriter
    {
        private readonly Encoding _shnEncoding;

        public 
            SHNBinaryWriter(Stream s, Encoding se) : base(s) => _shnEncoding = se;

        public void WriteString(string text, int length)
        {
            if (length == -1)
            {
                Write(_shnEncoding.GetBytes(text));

                Write((byte)0);
            }
            else
            {
                var stringBytes = _shnEncoding.GetBytes(text);
                var destinationArray = new byte[length];

                Array.Copy(stringBytes, destinationArray, Math.Min(length, stringBytes.Length));

                Write(destinationArray);
            }
        }
    }
}
