﻿using System;
using System.IO;
using System.Text;
using VisionLib.Common.Extensions;
using VisionLib.Common.Logging;
using VisionLib.Core.Stream;

namespace VisionLib.Common.Networking.Packet
{
    /// <summary>
	/// Class that represents a network message sent to and from a
	/// <see cref="FiestaNetConnection"/>
	/// </summary>
    public class FiestaNetPacket : VisionObject
    {
        /// <summary>
		/// The type of the message.
		/// </summary>
        public FiestaNetCommand Command { get; set; }

		/// <summary>
		/// Object to read data from the message.
		/// </summary>
		private readonly BinaryReader _reader;

		/// <summary>
		/// The stream that contains the message data.
		/// </summary>
		private readonly MemoryStream _stream;

		/// <summary>
		/// Object to write data to the message.
		/// </summary>
		private readonly BinaryWriter _writer;

		/// <summary>
		/// Creates a new instance of the <see cref="FiestaNetPacket"/> class.
		/// </summary>
		/// <param name="buffer">The message data.</param>
		public FiestaNetPacket(byte[] buffer)
		{
			var rs = new ReaderStream(buffer).Dump();
            // var rsDump = rs.Dump();
			_stream = new MemoryStream(buffer);
			_reader = new BinaryReader(_stream);
			Command = (FiestaNetCommand)ReadUInt16();
		}

		/// <summary>
		/// Creates a new instance of the <see cref="FiestaNetPacket"/> class.
		/// </summary>
		/// <param name="command">The type of message.</param>
		public FiestaNetPacket(FiestaNetCommand command)
		{
			_stream = new MemoryStream();
			_writer = new BinaryWriter(_stream);
			Command = command;

			Write((ushort)command);
		}

		/// <summary>
		/// Sends multiple messages as a chunk.
		/// </summary>
		public static void SendChunk(FiestaNetConnection connection, params FiestaNetPacket[] packets)
		{
			connection.SendChunk = true;

			for (var i = 0; i < packets.Length; i++)
			{
				packets[i].Send(connection);
			}

			connection.SendChunk = false;
		}

		/// <summary>
		/// Fills the number of bytes with the value.
		/// </summary>
		/// <param name="count">The number of bytes to fill.</param>
		/// <param name="value">The value to fill with.</param>
		public void Fill(int count, byte value)
		{
			for (var i = 0; i < count; i++)
			{
				Write(value);
			}
		}

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
		public bool ReadBoolean()
		{
			return _reader.ReadBoolean();
		}

		/// <summary>
		/// Reads a byte value from the current stream.
		/// </summary>
		public byte ReadByte()
		{
			return _reader.ReadByte();
		}

		/// <summary>
		/// Reads an array of bytes from the current stream.
		/// </summary>
		/// <param name="count">The number of bytes to read.</param>
		public byte[] ReadBytes(int count)
		{
			return _reader.ReadBytes(count);
		}

		/// <summary>
		/// Reads a character value from the current stream.
		/// </summary>
		public char ReadChar()
		{
			return _reader.ReadChar();
		}

		/// <summary>
		/// Reads a decimal value from the current stream.
		/// </summary>
		public decimal ReadDecimal()
		{
			return _reader.ReadDecimal();
		}

		/// <summary>
		/// Reads a double value from the current stream.
		/// </summary>
		public double ReadDouble()
		{
			return _reader.ReadDouble();
		}

		/// <summary>
		/// Reads a 16-bit integer value from the current stream.
		/// </summary>
		public short ReadInt16()
		{
			return _reader.ReadInt16();
		}

		/// <summary>
		/// Reads a 32-bit integer value from the current stream.
		/// </summary>
		public int ReadInt32()
		{
			return _reader.ReadInt32();
		}

		/// <summary>
		/// Reads a 64-bit integer value from the current stream.
		/// </summary>
		/// <returns></returns>
		public long ReadInt64()
		{
			return _reader.ReadInt64();
		}

		/// <summary>
		/// Reads a float value from the current stream.
		/// </summary>
		/// <returns></returns>
		public float ReadSingle()
		{
			return _reader.ReadSingle();
		}

		/// <summary>
		/// Reads a string value from the current stream.
		/// </summary>
		public string ReadString()
		{
			return ReadString(ReadByte());
		}

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
		public ushort ReadUInt16()
		{
			return _reader.ReadUInt16();
		}

		/// <summary>
		/// Reads an unsigned 32-bit integer value from the current stream.
		/// </summary>
		public uint ReadUInt32()
		{
			return _reader.ReadUInt32();
		}

		/// <summary>
		/// Reads an unsigned 64-bit integer value from the current stream.
		/// </summary>
		public ulong ReadUInt64()
		{
			return _reader.ReadUInt64();
		}

		#endregion

		/// <summary>
		/// Sends the message to the connection.
		/// </summary>
		/// <param name="connection">The connection to send the message to.</param>
		public void Send(FiestaNetConnection connection)
		{
			connection?.SendData(ToArray(connection));
            var endpointStr = connection == null ? "Connection Null" : connection.RemoteEndPoint.ToSimpleString();
            Log.Write(LogType.SocketLog, LogLevel.Debug, $"Sent {Command} packet to endpoint {endpointStr}");
            Destroy(this);
		}

		/// <summary>
		/// Returns a byte array representing the message.
		/// </summary>
		/// <returns></returns>
		public byte[] ToArray(FiestaNetConnection connection = null)
		{
			byte[] ret;
			var buffer = _stream.ToArray();

			if (connection != null)
			{
				// TODO: fix??
				var isClient = connection.ReceiveDestinationType.IsClient();
				var isToClient = connection.TransmitDestinationType.IsClient();
				while (!connection.Crypto.WasSeedSet()) { }
				if (isClient && !isToClient && connection.IsEstablished)
				{
					connection.Crypto.XorBuffer(buffer, 0, buffer.Length);
				}
			}

			if (buffer.Length <= 0xff)
			{
				ret = new byte[buffer.Length + 1];

				Buffer.BlockCopy(buffer, 0, ret, 1, buffer.Length);
				ret[0] = (byte)buffer.Length;
			}
			else
			{
				ret = new byte[buffer.Length + 3];

				Buffer.BlockCopy(buffer, 0, ret, 3, buffer.Length);
				Buffer.BlockCopy(BitConverter.GetBytes((ushort)buffer.Length), 0, ret, 1, 2);
			}

			return ret;
		}

		/// <summary>
		/// Returns a string representing the message.
		/// </summary>
		public override string ToString()
		{
			return $"Command=0x{Command:X} ({Command}), Length={_stream.Length - 2}";
		}

		#region Write

		/// <summary>
		/// Writes a boolean value to the current stream.
		/// </summary>
		/// <param name="value">The value to write.</param>
		public void Write(bool value)
		{
			_writer.Write(value);
		}

		/// <summary>
		/// Writes a byte value to the current stream.
		/// </summary>
		/// <param name="value">The value to write.</param>
		public void Write(byte value)
		{
			_writer.Write(value);
		}

		/// <summary>
		/// Writes a signed byte value to the current stream.
		/// </summary>
		/// <param name="value">The value to write.</param>
		public void Write(sbyte value)
		{
			_writer.Write(value);
		}

		/// <summary>
		/// Writes a byte array to the current stream.
		/// </summary>
		/// <param name="value">The value to write.</param>
		public void Write(byte[] value)
		{
			_writer.Write(value);
		}

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
		public void Write(short value)
		{
			_writer.Write(value);
		}

		/// <summary>
		/// Writes a 32-bit integer value to the current stream.
		/// </summary>
		/// <param name="value">The value to write.</param>
		public void Write(int value)
		{
			_writer.Write(value);
		}

		/// <summary>
		/// Writes a 64-bit integer value to the current stream.
		/// </summary>
		/// <param name="value">The value to write.</param>
		public void Write(long value)
		{
			_writer.Write(value);
		}

		/// <summary>
		/// Writes an unsigned 16-bit integer value to the current stream.
		/// </summary>
		/// <param name="value">The value to write.</param>
		public void Write(ushort value)
		{
			_writer.Write(value);
		}

		/// <summary>
		/// Writes an unsigned 32-bit integer value to the current stream.
		/// </summary>
		/// <param name="value">The value to write.</param>
		public void Write(uint value)
		{
			_writer.Write(value);
		}

		/// <summary>
		/// Writes an unsigned 64-bit integer value to the current stream.
		/// </summary>
		/// <param name="value">The value to write.</param>
		public void Write(ulong value)
		{
			_writer.Write(value);
		}

		/// <summary>
		/// Writes a double value to the current stream.
		/// </summary>
		/// <param name="value">The value to write.</param>
		public void Write(double value)
		{
			_writer.Write(value);
		}

		/// <summary>
		/// Writes a decimal value to the current stream.
		/// </summary>
		/// <param name="value">The value to write.</param>
		public void Write(decimal value)
		{
			_writer.Write(value);
		}

		/// <summary>
		/// Writes a float value to the current stream.
		/// </summary>
		/// <param name="value">The value to write.</param>
		public void Write(float value)
		{
			_writer.Write(value);
		}

		/// <summary>
		/// Writes a string value to the current stream.
		/// </summary>
		/// <param name="value">The value to write.</param>
		public void Write(string value)
		{
			Write(value, value.Length);
		}

		/// <summary>
		/// Writes a string value to the current stream.
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
		/// Destroys the <see cref="FiestaNetPacket"/> instance.
		/// </summary>
		protected override void Destroy()
		{
			// Reader and writer are not always initialized, so we need to check
			// for null before attempting to close them.
			_reader?.Close();
			_writer?.Close();
			_stream.Close();
		}
	}
}
