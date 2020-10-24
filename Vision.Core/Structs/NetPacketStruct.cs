using System.Threading.Tasks;
using Vision.Core.Networking;
using Vision.Core.Networking.Packet;

namespace Vision.Core.Structs
{
    public abstract class NetPacketStruct : AbstractStruct
    {
        public abstract NetCommand GetCommand();

        public void Read(NetPacket packet)
        {
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
    }
}