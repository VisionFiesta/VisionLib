using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Vision.Client.Services
{
    public sealed class MapService : ClientServiceBase
    {

        public MapService(FiestaClient client) : base(client)
        {
            var watch = Stopwatch.StartNew();
            ClientLogger.Debug("Initializing...");
            
            watch.Stop();
            ClientLogger.Info($"Initialized in {watch.Elapsed.TotalMilliseconds:0.####}ms");
        }

        private const int MapLoadTimeMillis = 500;

        public async Task LoadMap(string mapName, Action<double> onProgressCallback, Action<double> onCompleteCallback)
        {
            var mapLoadWatch = new Stopwatch();

            ClientLogger.Info($"Loading map \"{mapName}\"");

            mapLoadWatch.Start();
            for (var i = 0; i < 100; i++)
            {
                await Task.Delay(MapLoadTimeMillis / 100);
                await Task.Run(() => onProgressCallback(i));
            }
            mapLoadWatch.Stop();
            var loadTimeMillis = mapLoadWatch.Elapsed.TotalMilliseconds;

            await Task.Run(() => onCompleteCallback(loadTimeMillis));

            ClientLogger.Info($"Map loaded in {loadTimeMillis}");
        }
    }
}
