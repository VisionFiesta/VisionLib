using System.IO;
using System.Text;
using VisionLib.Common;
using VisionLib.Core.Dump;

namespace VisionLib.Core.Stream
{
    public class ReaderStream : VisionObject
    {
        private readonly MemoryStream _stream;
        private readonly BinaryReader _reader;

        public ReaderStream(ref MemoryStream stream)
        {
            _stream = stream;
			_reader = new BinaryReader(_stream);
        }

        public ReaderStream(byte[] buffer)
        {
            _stream = new MemoryStream(buffer, 0, buffer.Length, true, true);
            _reader = new BinaryReader(_stream);
        }

        public void SetOffset(long offset) => _stream.Position = offset;

        public long GetOffset() => _stream.Position;

        public long GetSize() => _stream.Length;

        /// <summary>
        /// The number of unread bytes in the message.
        /// </summary>
        public int RemainingBytes
        {
            get
            {
                var length = _stream?.Length;
                var position = _stream?.Position;
                return (int)((length.HasValue & position.HasValue ? length.GetValueOrDefault() - position.GetValueOrDefault() : new long?()) ?? 0L);
            }
        }

		#region Read

		/// <summary>
		/// Reads a boolean value from the current stream.
		/// </summary>
		public bool ReadBoolean() => _reader.ReadBoolean();

        /// <summary>
        /// Reads a byte value from the current stream.
        /// </summary>
        public byte ReadByte() => _reader.ReadByte();

        /// <summary>
		/// Reads an array of bytes from the current stream.
		/// </summary>
		/// <param name="count">The number of bytes to read.</param>
		public byte[] ReadBytes(int count) => _reader.ReadBytes(count);

		/// <summary>
		/// Reads a character value from the current stream.
		/// </summary>
		public char ReadChar() => _reader.ReadChar();

        /// <summary>
		/// Reads a decimal value from the current stream.
		/// </summary>
		public decimal ReadDecimal() => _reader.ReadDecimal();

        /// <summary>
		/// Reads a double value from the current stream.
		/// </summary>
		public double ReadDouble() => _reader.ReadDouble();

        /// <summary>
		/// Reads a 16-bit integer value from the current stream.
		/// </summary>
		public short ReadInt16() => _reader.ReadInt16();

        /// <summary>
		/// Reads a 32-bit integer value from the current stream.
		/// </summary>
		public int ReadInt32() => _reader.ReadInt32();

        /// <summary>
		/// Reads a 64-bit integer value from the current stream.
		/// </summary>
		/// <returns></returns>
		public long ReadInt64() => _reader.ReadInt64();

        /// <summary>
		/// Reads a float value from the current stream.
		/// </summary>
		/// <returns></returns>
		public float ReadFloat() => _reader.ReadSingle();

        /// <summary>
		/// Reads a string value from the current stream.
		/// </summary>
		public string ReadString() => ReadString(ReadByte());

        /// <summary>
		/// Reads a string value from the current stream.
		/// </summary>
		/// <param name="length">The length of the stream.</param>
		public string ReadString(int length)
		{
			var ret = string.Empty;
			var buffer = new byte[length];
			var count = 0;

			_stream.Read(buffer, 0, buffer.Length);

			if (buffer[length - 1] != 0)
			{
				count = length;
			}
			else
			{
				while (buffer[count] != 0 && count < length)
				{
					count++;
				}
			}

			if (count > 0)
			{
				ret = Encoding.ASCII.GetString(buffer, 0, count);
			}

			return ret;
		}

		/// <summary>
		/// Reads an unsigned 16-bit integer value from the current stream.
		/// </summary>
		public ushort ReadUInt16() => _reader.ReadUInt16();

        /// <summary>
		/// Reads an unsigned 32-bit integer value from the current stream.
		/// </summary>
		public uint ReadUInt32() => _reader.ReadUInt32();

        /// <summary>
		/// Reads an unsigned 64-bit integer value from the current stream.
		/// </summary>
		public ulong ReadUInt64() => _reader.ReadUInt64();

        #endregion

		public string Dump()
        {
			// hold position for reset after 
            var origPos = _stream.Position;
            _stream.Position = 0;

            var buf = (byte[])_stream.GetBuffer().Clone();

            var dmp = HexDump.Dump(buf);

            _stream.Position = origPos;

            return dmp;
        }

        /// <summary>
        /// Destroys the <see cref="ReaderStream"/> instance.
        /// </summary>
        protected override void Destroy()
        {
            _reader?.Close();
            _stream.Close();
        }
	}
}
