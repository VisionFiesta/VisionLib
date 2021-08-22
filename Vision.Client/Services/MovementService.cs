using System.Diagnostics;
using Vision.Core.Networking;
using Vision.Core.Networking.Packet;

namespace Vision.Client.Services
{
    public class MovementService : ClientServiceBase
    {
        public MovementService(FiestaClient client) : base(client)
        {
            var watch = Stopwatch.StartNew();
            ClientLogger.Debug("Initializing...");
            
            watch.Stop();
            ClientLogger.Info($"Initialized in {watch.Elapsed.TotalMilliseconds:0.####}ms");
        }


        private static readonly NetPacket JumpPacket = new(NetCommand.NC_ACT_JUMP_CMD);

        public void Jump()
        {
            JumpPacket.Send(ZoneConnection);
        }
    }
}
