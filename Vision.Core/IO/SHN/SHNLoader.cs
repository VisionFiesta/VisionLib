using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using Vision.Core.Extensions;
using Vision.Core.Logging.Loggers;

namespace Vision.Core.IO.SHN
{
    public class SHNLoader
    {
        private readonly SHNType _type;

        public SHNLoader(SHNType type) => _type = type;

        public delegate void SHNProcessor(SHNResult result, int index);

        private readonly Queue<Tuple<EngineLogLevel, string>> _messageQueue = new Queue<Tuple<EngineLogLevel, string>>();

        public void QueueMessage(EngineLogLevel level, string message) => _messageQueue.Enqueue(new Tuple<EngineLogLevel, string>(level, message));

        public void Load(SHNProcessor shnProcFunc)
        {
            var progressBar = EngineLog.CreateProgressBar($"Loading {_type}.shn", Color.SteelBlue);
            var watch = Stopwatch.StartNew();

            SHNManager.Load(_type, out var shnResult);

            progressBar.Update(0);
            for (var i = 0; i < shnResult.Count; i++)
            {
                shnProcFunc.Invoke(shnResult, i);
                
                var raw = (decimal)(0.5f + ((100f * i) / shnResult.Count));
                var percent = Math.Round(raw, MidpointRounding.ToEven);

                progressBar.Update((int)percent);
            }
            watch.Stop();
            var cmpMessage =
                $"Loaded {shnResult.Count} entries from {_type}.shn in {watch.Elapsed.TotalMilliseconds}ms";
            progressBar.Complete(cmpMessage, Color.SteelBlue);


            _messageQueue.DequeueAll(tuple => { EngineLog.WriteLine(tuple.Item1, tuple.Item2); });
        }
    }
}
