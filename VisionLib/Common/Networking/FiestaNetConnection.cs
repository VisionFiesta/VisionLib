using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using VisionLib.Common.Extensions;
using VisionLib.Common.Logging;
using VisionLib.Common.Utils;

namespace VisionLib.Common.Networking
{
    public class FiestaNetConnection : VisionObject
    {
        /// <summary>
        /// The maximum buffer size allowed.
        /// </summary>
        private const int ReceiveBufferSize = ushort.MaxValue;

        /// <summary>
        /// The crypto to use for this connection
        /// </summary>
        internal IFiestaNetCrypto Crypto;

        /// <summary>
        /// The connection's unique identifier.
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// The client's handle.
        /// </summary>
        public ushort Handle { get; set; }

        /// <summary>
        /// Returns true if the connection exists.
        /// </summary>
        public bool IsConnected => GetConnectionState();

        /// <summary>
        /// Returns true if the handshake has been completed.
        /// </summary>
        public bool IsEstablished { get; set; }

        /// <summary>
        /// Returns true if the connection is currently being sent data in chunks.
        /// </summary>
        public bool SendChunk
        {
            get => sendChunk;
            set
            {
                sendChunk = value;

                if (!value)
                {
                    SendAwaitingBuffers();
                }
            }
        }

        /// <summary>
        /// The destination type of the connection.
        /// </summary>
        public FiestaNetConnDest DestinationType { get; }

        /// <summary>
        /// The direction type of the connection.
        /// </summary>
        public FiestaNetConnDir DirectionType { get; }

        /// <summary>
        /// Time since the connection's last heartbeat.
        /// </summary>
        public long LastPing { get; set; }

        /// <summary>
        /// List of buffers waiting to be sent.
        /// </summary>
        private volatile List<byte[]> awaitingBuffers;

        /// <summary>
        /// The buffer used for incoming data.
        /// </summary>
        private byte[] receiveBuffer;

        /// <summary>
        /// The stream used to read from the incoming data buffer.
        /// </summary>
        private MemoryStream receiveStream;

        /// <summary>
        /// Returns true if the connection is currently being sent data in chunks.
        /// </summary>
        private bool sendChunk;

        /// <summary>
        /// The socket to use for data transferring.
        /// </summary>
        private Socket socket;

        /// <summary>
        /// Gets the sockets remote endpoint IP address
        /// </summary>
        /// <returns>Remote endpoint IP address</returns>
        public string GetRemoteIP => (socket.RemoteEndPoint as IPEndPoint)?.Address.ToString();

        public IPEndPoint RemoteEndPoint => socket.RemoteEndPoint as IPEndPoint;

        /// <summary>
        /// Gets the socket's connection state.
        /// </summary>
        /// <returns>True if the socket is connected.</returns>
        private bool GetConnectionState()
        {
            // If the socket is already null, we've already called
            // Disconnect().
            if (socket == null || !socket.Connected)
            {
                return false;
            }

            var blocking = socket.Blocking;

            try
            {
                var tempBuffer = new byte[1];

                socket.Blocking = false;
                socket.Send(tempBuffer, 0, 0);

                // If the send fails, this line won't be reached because an exception
                // will be thrown.
                return true;
            }
            catch (SocketException e)
            {
                // 10035 == WSAEWOULDBLOCK
                var ret = e.NativeErrorCode == 10035;

                if (!ret)
                {
                    Disconnect();
                }

                return ret;
            }
            finally
            {
                if (socket != null)
                {
                    socket.Blocking = blocking;
                }
            }
        }

        public FiestaNetConnection(FiestaNetConnDest dest, FiestaNetConnDir dir, IFiestaNetCrypto crypto = null)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            DestinationType = dest;
            DirectionType = dir;
            IsEstablished = true;
            Guid = System.Guid.NewGuid().ToString().Replace("-", "");
            Handle = (ushort)MathUtils.Random(ushort.MaxValue);
            receiveStream = new MemoryStream();
            awaitingBuffers = new List<byte[]>();

            // default to 2020 NA crypto
            Crypto = Crypto == null ? new FiestaNetCrypto_NA2020() : crypto;
        }

        // TODO: FiestaNetworkServer ctor
        /*
        public FiestaNetworkConnection(FiestaNetworkClient client, Socket socket, FiestaNetworkConnectionType type)
        { 
            Client = client; 
            this.socket = socket; 
            Type = type; 
            Guid = System.Guid.NewGuid().ToString().Replace("-", ""); 
            Handle = (ushort)MathUtils.Random(ushort.MaxValue); 
            receiveStream = new MemoryStream(); 
            awaitingBuffers = new List<byte[]>(); 
         
            // wait on seed ack to do `seed`, `setSeed`, and `IsEstablished` 
            // somehow? or is this not needed at all, since we only Connect? 
        }
        */

        /// <summary>
        /// Destroys the connection.
        /// </summary>
        public void Disconnect()
        {
            // May not want to remove here, might want to handle disconnections
            // individually per server by checking the list for disconnected
            // clients.
            //Client?.Connections.Remove(this);
            // TODO: child class for ServerConnection and ClientConnection

            Log.Write(LogType.SocketLog, LogLevel.Info, $"Disconnected from target: {DestinationType.ToMessage()}, Endpoint: {RemoteEndPoint.ToSimpleString()}");
            socket?.Close(); // Close() will call Dispose() automatically for us.
            socket = null;
        }

        /// <summary>
        /// Sends data to the connection.
        /// </summary>
        /// <param name="buffer">The data to send.</param>
        public void SendData(byte[] buffer)
        {
            if (!IsConnected)
            {
                return;
            }

            if (sendChunk)
            {
                awaitingBuffers.Add(buffer);
                return;
            }

            var bytesToSend = buffer.Length;
            var bytesSent = 0;

            if (bytesToSend >= ReceiveBufferSize)
            {
                Log.Write(LogType.SocketLog, LogLevel.Debug, "Exceeded max message size while sending data to a connection.");
            }

            while (bytesSent < bytesToSend)
            {
                bytesSent += socket.Send(buffer, bytesSent, bytesToSend - bytesSent, SocketFlags.None);

                if (bytesSent <= bytesToSend) continue;
                Log.Write(LogType.SocketLog, LogLevel.Warning, $"BUFFER OVERFLOW OCCURRED - Sent {bytesSent - bytesToSend} bytes more than expected.");
                break;
            }
        }

        /// <summary>
        /// Begins to accept data from the connection.
        /// </summary>
        private void BeginReceivingData()
        {
            if (!IsConnected)
            {
                return;
            }

            receiveBuffer = new byte[ReceiveBufferSize];
            socket.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, ReceivedData, null);
        }

        /// <summary>
        /// Attempts to connect to the specified target endpoint.
        /// </summary>
        /// <param name="targetIP">The target IP Address.</param>
        /// <param name="targetPort">The target port.</param>
        public void Connect(string targetIP, ushort targetPort)
        {
            socket.BeginConnect(new IPEndPoint(IPAddress.Parse(targetIP), targetPort), ConnectedToTarget, new object[] { targetIP, targetPort });
        }

        /// <summary>
        /// This callback is called when we have connected to the requested
        /// target endpoint.
        /// </summary>
        /// <param name="e">The result object.</param>
        private void ConnectedToTarget(IAsyncResult e)
        {
            try
            {
                socket.EndConnect(e);
                Log.Write(LogType.SocketLog, LogLevel.Info, $"Connected to target: {DestinationType.ToMessage()}, Endpoint: {RemoteEndPoint.ToSimpleString()}");
                BeginReceivingData();
            }
            catch
            {
                Log.Write(LogType.SocketLog, LogLevel.Warning, "Remote socket connection attempt failed. Trying again...");
                Thread.Sleep(3000); // 3 seconds.
                Connect((string)((object[])e.AsyncState)[0], (ushort)((object[])e.AsyncState)[1]);
            }
        }

        /// <summary>
        /// Destroys the <see cref="NetworkConnection"/> instance.
        /// </summary>
        protected override void Destroy()
        {
            socket?.Close();
        }

        /// <summary>
        /// Gets all messages from the buffer.
        /// </summary>
        /// <param name="buffer">The buffer to get message from.</param>
        private void GetMessagesFromBuffer(byte[] buffer)
        {
            if (!IsConnected)
            {
                return;
            }

            receiveStream.Write(buffer, 0, buffer.Length);

            while (TryParseMessage())
            {

            }
        }

        /// <summary>
        /// Called when data was received from the connection.
        /// </summary>
        /// <param name="e">The status of the operation.</param>
        private void ReceivedData(IAsyncResult e)
        {
            if (!IsConnected)
            {
                return;
            }

            var count = socket.EndReceive(e);
            var buffer = new byte[count];

            if (count <= 0)
            {
                Disconnect();
                return;
            }

            Array.Copy(receiveBuffer, 0, buffer, 0, count);
            GetMessagesFromBuffer(buffer);

            BeginReceivingData();
        }

        /// <summary>
        /// Sends the awaiting buffers as one big chunk.
        /// </summary>
        private void SendAwaitingBuffers()
        {
            if (!IsConnected)
            {
                return;
            }

            awaitingBuffers.Copy(out var bufferList);
            awaitingBuffers.Clear();

            var size = bufferList.Sum(b => b.Length);
            var chunk = new byte[size];
            var pointer = 0;

            for (var i = 0; i < bufferList.Count; i++)
            {
                var buffer = bufferList[i];

                Array.Copy(buffer, 0, chunk, pointer, buffer.Length);
                pointer += buffer.Length;
            }

            SendData(chunk);
        }

        /// <summary>
        /// Tries to parse a message from the receive stream.
        /// </summary>
        private bool TryParseMessage()
        {
            if (!IsConnected)
            {
                return false;
            }

            receiveStream.Position = 0;

            if (receiveStream.Length < 1)
            {
                return false;
            }

            ushort messageSize;
            var sizeBuffer = new byte[1];

            receiveStream.Read(sizeBuffer, 0, 1);

            if (sizeBuffer[0] != 0)
            {
                messageSize = sizeBuffer[0];
            }
            else
            {
                if (receiveStream.Length - receiveStream.Position < 2)
                {
                    return false;
                }

                sizeBuffer = new byte[2];
                receiveStream.Read(sizeBuffer, 0, 2);

                messageSize = BitConverter.ToUInt16(sizeBuffer, 0);
            }

            if (receiveStream.Length - receiveStream.Position < messageSize)
            {
                return false;
            }

            var messageBuffer = new byte[messageSize];
            receiveStream.Read(messageBuffer, 0, messageSize);

            // TODO: is this correct?
            bool isFromClient = DirectionType.IsFromClient();
            bool isToServer = DestinationType.IsToServer();
            if (!isFromClient && !isToServer && Crypto.WasSeedSet())
            {
                Crypto.XorBuffer(messageBuffer, 0, messageBuffer.Length);
            }

            var packet = new FiestaNetPacket(messageBuffer);
            if (FiestaNetPacketHandlerLoader.TryGetHandler(packet.Command, out var handler))
            {
                var watch = new Stopwatch();
                watch.Start();
                handler(packet, this);
                watch.Stop();
                string millisString = string.Format("{0:N2}", watch.Elapsed.TotalMilliseconds);
                Log.Write(LogType.SocketLog, LogLevel.Debug, $"Handler for {packet.Command} took {millisString}ms to complete.");
            }
            else
            {
                Log.Write(LogType.SocketLog, LogLevel.Warning, $"Got unhandled command: {packet.Command}");
            }

            // Trims the receive stream.
            var remainingByteCount = new byte[receiveStream.Length - receiveStream.Position];
            receiveStream.Read(remainingByteCount, 0, remainingByteCount.Length);
            receiveStream = new MemoryStream();
            receiveStream.Write(remainingByteCount, 0, remainingByteCount.Length);

            return true;
        }
    }
}
