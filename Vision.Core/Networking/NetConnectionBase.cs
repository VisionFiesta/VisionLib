using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Vision.Core.Collections;
using Vision.Core.Extensions;
using Vision.Core.Logging.Loggers;
using Vision.Core.Networking.Crypto;
using Vision.Core.Networking.Packet;

namespace Vision.Core.Networking
{
    public abstract class NetConnectionBase<T> : VisionObject where T : NetConnectionBase<T>
    {
        private readonly SocketLog _logger;

        /// <summary>
        /// The maximum buffer size allowed.
        /// </summary>
        private const int ReceiveBufferSize = ushort.MaxValue;

        /// <summary>
        /// The crypto to use for this connection
        /// </summary>
        public INetCrypto Crypto { get; }

        /// <summary>
        /// Returns true if the connection exists.
        /// </summary>
        public bool IsConnected => GetConnectionState();

        /// <summary>
        /// Returns true if the handshake has been completed.
        /// </summary>
        public bool IsEstablished { get; }

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
        public NetConnectionDestination TransmitDestinationType { get; }

        /// <summary>
        /// The sender type of the connection.
        /// </summary>
        public NetConnectionDestination ReceiveDestinationType { get; }

        /// <summary>
        /// The region of the connection.
        /// </summary>
        public GameRegion Region { get; }

        // TODO: actually impl this
        /// <summary>
        /// Time of the connection's last heartbeat.
        /// </summary>
        public DateTime LastPing { get; set; }

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
        /// Returns the remote endpoint of the connection.
        /// </summary>
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

        protected NetConnectionBase(NetConnectionDestination txDest, NetConnectionDestination rxDest, GameRegion region = GameRegion.GR_NA, INetCrypto crypto = null)
        {
            _logger = new SocketLog(GetType());

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            TransmitDestinationType = txDest;
            ReceiveDestinationType = rxDest;
            Region = region;
            IsEstablished = true;
            _receiveStream = new MemoryStream();
            _awaitingBuffers = new List<byte[]>();

            // default to 2020 NA crypto TODO: handle region cryptos separate
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            Crypto = Crypto == null ? new NetCryptoNa2020() : crypto;
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

        public delegate void OnConnectCallback(NetConnectionDestination dest, IPEndPoint endPoint);
        public delegate void OnDisconnectCallback(NetConnectionDestination dest, IPEndPoint endPoint);

        private readonly FastList<OnConnectCallback> _connectCallbacks = new();
        private readonly FastList<OnDisconnectCallback> _disconnectCallbacks = new();

        public void AddConnectCallback(OnConnectCallback callback) => _connectCallbacks.Add(callback);

        public void AddDisconnectCallback(OnDisconnectCallback callback) => _disconnectCallbacks.Add(callback);

        /// <summary>
        /// Destroys the connection.
        /// </summary>
        public void Disconnect()
        {
            foreach (var dc in _disconnectCallbacks)
            {
                dc.Invoke(TransmitDestinationType, RemoteEndPoint);
            }

            _logger.Info($"Disconnected from target: {TransmitDestinationType.ToMessage()}, Endpoint: {RemoteEndPoint.ToSimpleString()}");
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
                _logger.Debug("Exceeded max message size while sending data to a connection.");
            }

            while (bytesSent < bytesToSend)
            {
                bytesSent += _socket.Send(buffer, bytesSent, bytesToSend - bytesSent, SocketFlags.None);

                if (bytesSent <= bytesToSend) continue;
                _logger.Warning($"BUFFER OVERFLOW OCCURRED - Sent {bytesSent - bytesToSend} bytes more than expected.");
                break;
            }
        }

        public void SendDataAsync(byte[] buffer, EventHandler<SocketAsyncEventArgs> completedCallback)
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

            if (bytesToSend >= ReceiveBufferSize)
            {
                _logger.Debug("Exceeded max message size while sending data to a connection.");
                return;
            }

            var e = new SocketAsyncEventArgs();
            e.SetBuffer(buffer);
            e.Completed += completedCallback;

            bool completedAsync;

            try
            {
                completedAsync = _socket.SendAsync(e);
                // await _socket.SendAsync(buffer, SocketFlags.None);
            }
            catch (SocketException ex)
            {
                _logger.Info($"Socket not prepared for send! {ex.Message}");
                return;
            }

            if (!completedAsync)
            {
                completedCallback.Invoke(this, e);
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

                _logger.Info($"Connected to target: {TransmitDestinationType.ToMessage()}, Endpoint: {RemoteEndPoint.ToSimpleString()}");
                BeginReceivingData();
            }
            catch
            {
                _logger.Warning("Remote socket connection attempt failed. Trying again...");
                Thread.Sleep(3000); // 3 seconds.
                Connect((IPEndPoint)e.AsyncState);
            }
        }

        /// <summary>
        /// Destroys the <see cref="NetConnectionBase{T}"/> instance.
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

            foreach (var buffer in bufferList)
            {
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

            _receiveStream.ReadAsync(sizeBuffer, 0, 1);

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

            var packet = new NetPacket(messageBuffer);

            GetAndRunHandler(packet, this as T);

            // Trims the receive stream.
            var remainingByteCount = new byte[_receiveStream.Length - _receiveStream.Position];
            _receiveStream.Read(remainingByteCount, 0, remainingByteCount.Length);
            _receiveStream = new MemoryStream();
            _receiveStream.Write(remainingByteCount, 0, remainingByteCount.Length);

            return true;
        }

        private void GetAndRunHandler(NetPacket packet, T connection)
        {
            var hasHandler =
                NetPacketHandlerLoader<T>.TryGetHandler(packet.Command, out var handler, out var destinations);
            var isForThisDest = destinations?.Contains(connection.ReceiveDestinationType) ?? false;

            switch (hasHandler)
            {
                case true when isForThisDest:
                {
                    if (!NetPacket.DebugSkipCommands.Contains(packet.Command))
                    {
                        var watch = Stopwatch.StartNew();
                        handler(packet, connection);
                        watch.Stop();
                        var millisString = $"{watch.Elapsed.TotalMilliseconds:N2}";
                        _logger.Debug($"Handler for {packet.Command} took {millisString}ms to complete.");
                    }
                    else
                    {
                        // quiet handle
                        handler(packet, connection);
                    }

                    break;
                }
                // not this dest
                case true:
                    _logger.Warning($"Got handled command from {connection.TransmitDestinationType} NOT FOR {connection.ReceiveDestinationType} : {packet.Command}");
                    break;
                default:
                {
                    var commandHex = $"0x{packet.Command:x}";
                    var commandName = Enum.GetName(typeof(NetCommand), packet.Command);
                    if (string.IsNullOrEmpty(commandName)) commandName = "Unknown";
                    _logger.Unhandled($"Got unhandled command from {connection.TransmitDestinationType}: {commandHex} | {commandName}");
                    break;
                }
            }
        }
    }
}
