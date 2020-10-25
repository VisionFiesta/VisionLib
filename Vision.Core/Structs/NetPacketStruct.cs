using System;
using System.Threading.Tasks;
using Vision.Core.Logging.Loggers;
using Vision.Core.Networking;
using Vision.Core.Networking.Packet;

namespace Vision.Core.Structs
{
    public abstract class NetPacketStruct : AbstractStruct
    {
        private static readonly EngineLog Logger = new EngineLog(typeof(NetPacketStruct));

        public abstract NetCommand GetCommand();

        public void Read(NetPacket packet)
        {
            var packetSizeDiff = packet.Reader.RemainingBytes - GetSize();

            if (packetSizeDiff < 0)
            {
                Logger.Error($"Packet smaller than expected by {Math.Abs(packetSizeDiff)} bytes for opcode {GetCommand()}");
                return;
            }

            if (packetSizeDiff > 0 && HasMaximumSize())
            {
                Logger.Warning($"Packet bigger than expected by {packetSizeDiff} bytes for opcode {GetCommand()}");
            }

            Read(packet.Reader);
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

        public virtual bool HasMaximumSize() => true;
    }
}