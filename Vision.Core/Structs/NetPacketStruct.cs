using System;
using System.IO;
using System.Threading.Tasks;
using Vision.Core.Logging.Loggers;
using Vision.Core.Networking;
using Vision.Core.Networking.Packet;

namespace Vision.Core.Structs
{
    public abstract class NetPacketStruct : AbstractStruct
    {
        private static readonly EngineLog Logger = new(typeof(NetPacketStruct));

        protected abstract NetCommand GetCommand();

        public void Read(NetPacket packet)
        {
            var packetSizeDiff = packet.Reader.RemainingBytes - GetSize();

            switch (packetSizeDiff)
            {
                case < 0:
                    Logger.Error($"Packet smaller than expected by {Math.Abs(packetSizeDiff)} bytes for opcode {GetCommand()}");
                    break;
                case > 0 when HasMaximumSize():
                    Logger.Warning($"Packet bigger than expected by {packetSizeDiff} bytes for opcode {GetCommand()}");
                    break;
            }

            try
            {
                Read(packet.Reader);

                var leftoverBytes = packet.Reader.RemainingBytes;
                if (leftoverBytes == 0) return;
                
                Logger.Warning($"Eating {leftoverBytes} extra bytes for opcode {GetCommand()}");
                // eat leftover bytes to ensure safe read of next struct
                packet.Reader.ReadBytes(packet.Reader.RemainingBytes);
            }
            catch (EndOfStreamException)
            {
                Logger.Warning("Packet read with invalid byte count, data may not be accurate!");
            }
        }

        public void Send<T>(T connection) where T : NetConnectionBase<T>
        {
            var pkt = new NetPacket(GetCommand());
            Write(pkt.Writer);
            pkt.Send(connection);
        }

        public async Task SendAsync<T>(T connection) where T : NetConnectionBase<T>
        {
            var pkt = new NetPacket(GetCommand());
            await Task.Run(() =>
            {
                Write(pkt.Writer);
                pkt.Send(connection);
            });
        }

        protected virtual bool HasMaximumSize() => true;
    }
}