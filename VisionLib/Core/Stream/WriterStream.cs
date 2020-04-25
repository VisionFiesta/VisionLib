using System.IO;
using System.Text;
using VisionLib.Common;

namespace VisionLib.Core.Stream
{
    public class WriterStream : VisionObject
    {
        private readonly MemoryStream _stream;
        private readonly BinaryWriter _writer;

        public WriterStream(int size) : this(new byte[size]) { }

        public WriterStream(long size) : this(new byte[(int)size]) { }

        public WriterStream(ref MemoryStream stream)
        {
            _stream = stream;
			_writer = new BinaryWriter(_stream);
        }

        public WriterStream(byte[] buffer)
        {
            _stream = new MemoryStream(buffer, 0, buffer.Length, true, true);
            _writer = new BinaryWriter(_stream);
        }

        public void SetOffset(long offset) => _stream.Position = offset;

        public long GetOffset() => _stream.Position;

        public byte[] GetBuffer() => _stream.GetBuffer();

		#region Write

		/// <summary>
		/// Writes a boolean value to the current stream.
		/// </summary>
		/// <param name="value">The value to write.</param>
		public void Write(bool value) => _writer.Write(value);

        /// <summary>
		/// Writes a byte value to the current stream.
		/// </summary>
		/// <param name="value">The value to write.</param>
		public void Write(byte value) => _writer.Write(value);

        /// <summary>
		/// Writes a signed byte value to the current stream.
		/// </summary>
		/// <param name="value">The value to write.</param>
		public void Write(sbyte value) => _writer.Write(value);

        /// <summary>
		/// Writes a byte array to the current stream.
		/// </summary>
		/// <param name="value">The value to write.</param>
		public void Write(byte[] value) => _writer.Write(value);

        /// <summary>
		/// Writes a byte array to the current stream, up to the supplied length.
		/// </summary>
		/// <param name="value">The value to write.</param>
		/// <param name="len">The amount of bytes to write</param>
		public void Write(byte[] value, int len)
		{
			for (var i = 0; i < len; i++)
			{
				_writer.Write(value[i]);
			}
		}

		/// <summary>
		/// Writes a 16-bit integer value to the current stream.
		/// </summary>
		/// <param name="value">The value to write.</param>
		public void Write(short value) => _writer.Write(value);

        /// <summary>
		/// Writes a 32-bit integer value to the current stream.
		/// </summary>
		/// <param name="value">The value to write.</param>
		public void Write(int value) => _writer.Write(value);

        /// <summary>
		/// Writes a 64-bit integer value to the current stream.
		/// </summary>
		/// <param name="value">The value to write.</param>
		public void Write(long value) => _writer.Write(value);

        /// <summary>
		/// Writes an unsigned 16-bit integer value to the current stream.
		/// </summary>
		/// <param name="value">The value to write.</param>
		public void Write(ushort value) => _writer.Write(value);

        /// <summary>
		/// Writes an unsigned 32-bit integer value to the current stream.
		/// </summary>
		/// <param name="value">The value to write.</param>
		public void Write(uint value) => _writer.Write(value);

        /// <summary>
		/// Writes an unsigned 64-bit integer value to the current stream.
		/// </summary>
		/// <param name="value">The value to write.</param>
		public void Write(ulong value) => _writer.Write(value);

        /// <summary>
		/// Writes a double value to the current stream.
		/// </summary>
		/// <param name="value">The value to write.</param>
		public void Write(double value) => _writer.Write(value);

        /// <summary>
		/// Writes a decimal value to the current stream.
		/// </summary>
		/// <param name="value">The value to write.</param>
		public void Write(decimal value) => _writer.Write(value);

        /// <summary>
		/// Writes a float value to the current stream.
		/// </summary>
		/// <param name="value">The value to write.</param>
		public void Write(float value) => _writer.Write(value);

        /// <summary>
		/// Writes a string value to the current stream.
		/// </summary>
		/// <param name="value">The value to write.</param>
		public void Write(string value) => Write(value, value.Length);

        /// <summary>
		/// Writes a padded string value to the current stream.
		/// </summary>
		/// <param name="value">The value to write.</param>
		/// <param name="length">The length of the string.</param>
		public void Write(string value, int length)
		{
			var buffer = Encoding.ASCII.GetBytes(value);

			Write(buffer);

			for (var i = 0; i < length - buffer.Length; i++)
			{
				Write((byte)0);
			}
		}

        #endregion

		/// <summary>
		/// Destroys the <see cref="WriterStream"/> instance.
		/// </summary>
		protected override void Destroy()
        {
            _writer?.Close();
            _stream?.Close();
        }
	}
}
