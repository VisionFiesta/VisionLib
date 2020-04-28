using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using VisionLib.Common.Collections;
using VisionLib.Common.Extensions;
using VisionLib.Common.Logging;
using VisionLib.Common.Networking.Crypto;
using VisionLib.Common.Networking.Packet;
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
            get => _sendChunk;
            set
            {
                _sendChunk = value;

                if (!value)
                {
                    SendAwaitingBuffers();
                }
            }
        }

        /// <summary>
        /// The destination type of the connection.
        /// </summary>
        public FiestaNetConnDest TransmitDestinationType { get; }

        /// <summary>
        /// The sender type of the connection.
        /// </summary>
        public FiestaNetConnDest ReceiveDestinationType { get; }

        /// <summary>
        /// Time since the connection's last heartbeat.
        /// </summary>
        public long LastPing { get; set; }

        /// <summary>
        /// List of buffers waiting to be sent.
        /// </summary>
        private volatile List<byte[]> _awaitingBuffers;

        /// <summary>
        /// The buffer used for incoming data.
        /// </summary>
        private byte[] _receiveBuffer;

        /// <summary>
        /// The stream used to read from the incoming data buffer.
        /// </summary>
        private MemoryStream _receiveStream;

        /// <summary>
        /// Returns true if the connection is currently being sent data in chunks.
        /// </summary>
        private bool _sendChunk;

        /// <summary>
        /// The socket to use for data transferring.
        /// </summary>
        private Socket _socket;

        /// <summary>
        /// Gets the sockets remote endpoint IP address
        /// </summary>
        /// <returns>Remote endpoint IP address</returns>
        public string GetRemoteIp => (_socket.RemoteEndPoint as IPEndPoint)?.Address.ToString();

        public IPEndPoint RemoteEndPoint => _socket.RemoteEndPoint as IPEndPoint;

        /// <summary>
        /// Gets the socket's connection state.
        /// </summary>
        /// <returns>True if the socket is connected.</returns>
        private bool GetConnectionState()
        {
            // If the socket is already null, we've already called
            // Disconnect().
            if (_socket == null || !_socket.Connected)
            {
                return false;
            }

            var blocking = _socket.Blocking;

            try
            {
                var tempBuffer = new byte[1];

                _socket.Blocking = false;
                _socket.Send(tempBuffer, 0, 0);

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
                if (_socket != null)
                {
                    _socket.Blocking = blocking;
                }
            }
        }

        public FiestaNetConnection(FiestaNetConnDest txDest, FiestaNetConnDest rxDest, IFiestaNetCrypto crypto = null)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            TransmitDestinationType = txDest;
            ReceiveDestinationType = rxDest;
            IsEstablished = true;
            Guid = System.Guid.NewGuid().ToString().Replace("-", "");
            Handle = (ushort)MathUtils.Random(ushort.MaxValue);
            _receiveStream = new MemoryStream();
            _awaitingBuffers = new List<byte[]>();

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

        public delegate void OnConnectCallback(FiestaNetConnDest dest, IPEndPoint endPoint);
        public delegate void OnDisconnectCallback(FiestaNetConnDest dest, IPEndPoint endPoint);

        private FastList<OnConnectCallback> _connectCallbacks = new FastList<OnConnectCallback>();
        private FastList<OnDisconnectCallback> _disconnectCallbacks = new FastList<OnDisconnectCallback>();

        public void AddConnectCallbact(OnConnectCallback callback) => _connectCallbacks.Add(callback);

        public void AddDisconnectCallback(OnDisconnectCallback callback) => _disconnectCallbacks.Add(callback);

        /// <summary>
        /// Destroys the connection.
        /// </summary>
        public void Disconnect()
        {
            // May not want to remove here, might want to handle disconnections
            // individually per server by checking the list for disconnected
            // clients.
            //Client?.Connections.Remove(this);
            // TODO: child class for ServerConnection and ClientConnection?

            foreach (var dc in _disconnectCallbacks)
            {
                dc.Invoke(TransmitDestinationType, RemoteEndPoint);
            }

            SocketLog.Info( $"Disconnected from target: {TransmitDestinationType.ToMessage()}, Endpoint: {RemoteEndPoint.ToSimpleString()}");
            _socket?.Close(); // Close() will call Dispose() automatically for us.
            _socket = null;
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

            if (_sendChunk)
            {
                _awaitingBuffers.Add(buffer);
                return;
            }

            var bytesToSend = buffer.Length;
            var bytesSent = 0;

            if (bytesToSend >= ReceiveBufferSize)
            {
                SocketLog.Debug("Exceeded max message size while sending data to a connection.");
            }

            while (bytesSent < bytesToSend)
            {
                bytesSent += _socket.Send(buffer, bytesSent, bytesToSend - bytesSent, SocketFlags.None);

                if (bytesSent <= bytesToSend) continue;
                SocketLog.Warning($"BUFFER OVERFLOW OCCURRED - Sent {bytesSent - bytesToSend} bytes more than expected.");
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

            _receiveBuffer = new byte[ReceiveBufferSize];
            _socket.BeginReceive(_receiveBuffer, 0, _receiveBuffer.Length, SocketFlags.None, ReceivedData, null);
        }

        /// <summary>
        /// Attempts to connect to the specified target endpoint.
        /// </summary>
        /// <param name="endPoint">The target endpoint.</param>
        public void Connect(IPEndPoint endPoint)
        {
            _socket.BeginConnect(endPoint, ConnectedToTarget, endPoint);
        }

        /// <summary>
        /// Attempts to connect to the specified target endpoint.
        /// </summary>
        /// <param name="targetIP">The target IP Address.</param>
        /// <param name="targetPort">The target port.</param>
        public void Connect(string targetIP, ushort targetPort)
        {
           Connect(new IPEndPoint(IPAddress.Parse(targetIP), targetPort));
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
                _socket.EndConnect(e);

                foreach (var c in _connectCallbacks)
                {
                    c.Invoke(TransmitDestinationType, RemoteEndPoint);
                }

                SocketLog.Info( $"Connected to target: {TransmitDestinationType.ToMessage()}, Endpoint: {RemoteEndPoint.ToSimpleString()}");
                BeginReceivingData();
            }
            catch
            {
                SocketLog.Warning("Remote socket connection attempt failed. Trying again...");
                Thread.Sleep(3000); // 3 seconds.
                Connect((IPEndPoint)e.AsyncState);
            }
        }

        /// <summary>
        /// Destroys the <see cref="FiestaNetConnection"/> instance.
        /// </summary>
        protected override void Destroy()
        {
            _socket?.Close();
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

            _receiveStream.Write(buffer, 0, buffer.Length);

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

            var count = _socket.EndReceive(e);
            var buffer = new byte[count];

            if (count <= 0)
            {
                Disconnect();
                return;
            }

            Array.Copy(_receiveBuffer, 0, buffer, 0, count);
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

            _awaitingBuffers.Copy(out var bufferList);
            _awaitingBuffers.Clear();

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

            _receiveStream.Position = 0;

            if (_receiveStream.Length < 1)
            {
                return false;
            }

            ushort messageSize;
            var sizeBuffer = new byte[1];

            _receiveStream.Read(sizeBuffer, 0, 1);

            if (sizeBuffer[0] != 0)
            {
                messageSize = sizeBuffer[0];
            }
            else
            {
                if (_receiveStream.Length - _receiveStream.Position < 2)
                {
                    return false;
                }

                sizeBuffer = new byte[2];
                _receiveStream.Read(sizeBuffer, 0, 2);

                messageSize = BitConverter.ToUInt16(sizeBuffer, 0);
            }

            if (_receiveStream.Length - _receiveStream.Position < messageSize)
            {
                return false;
            }

            var messageBuffer = new byte[messageSize];
            _receiveStream.Read(messageBuffer, 0, messageSize);

            var isFromClient = ReceiveDestinationType.IsClient();
            var isToServer = TransmitDestinationType.IsServer();
            if (!isFromClient && !isToServer && Crypto.WasSeedSet())
            {
                Crypto.XorBuffer(messageBuffer, 0, messageBuffer.Length);
            }

            var packet = new FiestaNetPacket(messageBuffer);

            var hasHandler =
                FiestaNetPacketHandlerLoader.TryGetHandler(packet.Command, out var handler, out var destinations);
            var isForThisDest = destinations?.Contains(ReceiveDestinationType) ?? false;

            if (hasHandler && isForThisDest)
            {
                if (!FiestaNetPacket.DebugSkipCommands.Contains(packet.Command))
                {
                    var watch = new Stopwatch();
                    watch.Start();
                    handler(packet, this);
                    watch.Stop();
                    var millisString = $"{watch.Elapsed.TotalMilliseconds:N2}";
                    SocketLog.Debug($"Handler for {packet.Command} took {millisString}ms to complete.");
                }
                else
                {
                    handler(packet, this);
                }
            }
            else if (hasHandler) // not this dest
            {
                SocketLog.Warning($"Got handled command from {TransmitDestinationType} NOT FOR {ReceiveDestinationType} : {packet.Command}");
            }
            else
            {
                SocketLog.Unhandled($"Got unhandled command from {TransmitDestinationType}: {packet.Command}");
            }

            // Trims the receive stream.
            var remainingByteCount = new byte[_receiveStream.Length - _receiveStream.Position];
            _receiveStream.Read(remainingByteCount, 0, remainingByteCount.Length);
            _receiveStream = new MemoryStream();
            _receiveStream.Write(remainingByteCount, 0, remainingByteCount.Length);

            return true;
        }
    }
}
