using System;
using System.IO;
using System.Linq;
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
        public static readonly FiestaNetCommand[] DebugSkipCommands =
        {
            FiestaNetCommand.NC_MISC_HEARTBEAT_REQ,
            FiestaNetCommand.NC_MISC_HEARTBEAT_ACK,
			FiestaNetCommand.NC_BRIEFINFO_ABSTATE_CHANGE_CMD,
			FiestaNetCommand.NC_ACT_SOMEONEMOVERUN_CMD,
			FiestaNetCommand.NC_ACT_SOMEONEMOVEWALK_CMD,
			FiestaNetCommand.NC_ACT_SOMEONESTOP_CMD,
			FiestaNetCommand.NC_BAT_ABSTATESET_CMD,
			FiestaNetCommand.NC_BAT_CEASE_FIRE_CMD,
		};

        /// <summary>
		/// The type of the message.
		/// </summary>
        public FiestaNetCommand Command { get; set; }

        /// <summary>
        /// Object to read data from the message.
        /// </summary>
        public ReaderStream Reader { get; }

		/// <summary>
		/// The stream that contains the message data.
		/// </summary>
		private readonly MemoryStream _stream;

		/// <summary>
		/// Object to write data to the message.
		/// </summary>
		public WriterStream Writer { get; }

        public int Size { get; protected set; }

		/// <summary>
		/// Creates a new instance of the <see cref="FiestaNetPacket"/> class.
		/// </summary>
		/// <param name="buffer">The message data.</param>
		public FiestaNetPacket(byte[] buffer)
		{
            _stream = new MemoryStream(buffer);
            Reader = new ReaderStream(ref _stream);
			Writer = new WriterStream(ref _stream);
			
			Command = (FiestaNetCommand)Reader.ReadUInt16();
            Size = Reader.RemainingBytes;
        }

		/// <summary>
		/// Creates a new instance of the <see cref="FiestaNetPacket"/> class.
		/// </summary>
		/// <param name="command">The type of message.</param>
		public FiestaNetPacket(FiestaNetCommand command)
		{
			_stream = new MemoryStream();
			Writer = new WriterStream(ref _stream);
			Command = command;

			Writer.Write((ushort)command);
		}

		/// <summary>
		/// Sends multiple messages as a chunk.
		/// </summary>
		public static void SendChunk(FiestaNetConnection connection, params FiestaNetPacket[] packets)
		{
			connection.SendChunk = true;

			foreach (var pkt in packets)
            {
                pkt.Send(connection);
            }

			connection.SendChunk = false;
		}

        /// <summary>
		/// Sends the message to the connection.
		/// </summary>
		/// <param name="connection">The connection to send the message to.</param>
		public void Send(FiestaNetConnection connection)
        {
			connection?.SendData(ToArray(connection));

            if (!DebugSkipCommands.Contains(Command))
            {
                var endpointStr = connection == null ? "Connection Null" : connection.RemoteEndPoint.ToSimpleString();
                SocketLog.Debug($"Sent {Command} packet to endpoint {endpointStr}, Size: {Size}");
			}

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

		/// <summary>
		/// Destroys the <see cref="FiestaNetPacket"/> instance.
		/// </summary>
		protected override void Destroy()
		{
			Destroy(Reader);
			Destroy(Writer);
			_stream?.Close();
		}
	}
}
