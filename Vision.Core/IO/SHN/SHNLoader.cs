using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using Vision.Core.Extensions;
using Vision.Core.Logging.Loggers;

namespace Vision.Core.IO.SHN
{
    public class SHNLoader
    {
        private readonly string _shnFolder;
        private readonly SHNType _type;
        private readonly ISHNCrypto _crypto;

        protected internal SHNLoader(string shnFolder, SHNType type, ISHNCrypto crypto)
        {
            _shnFolder = shnFolder;
            _type = type;
            _crypto = crypto;
        }

        public delegate void SHNProcessor(SHNResult result, int index);

        private readonly Queue<Tuple<EngineLogLevel, string>> _messageQueue = new Queue<Tuple<EngineLogLevel, string>>();

        public void QueueMessage(EngineLogLevel level, string message) => _messageQueue.Enqueue(new Tuple<EngineLogLevel, string>(level, message));

        private bool Load(out SHNResult shnResult)
        {
            shnResult = new SHNResult();

            var encoding = _type == SHNType.TextData ? Encoding.ASCII : Encoding.GetEncoding("ISO-8859-1");

            var shnFile = SHNFile.Create(_shnFolder, _type, _crypto, encoding);
            if (shnFile == null) return false;
            shnFile.Read();
            shnFile.DisallowRowChanges();

            shnResult.Load(shnFile);
            return true;
        }

        public void Load(SHNProcessor shnProcFunc)
        {
            var progressBar = EngineLog.CreateProgressBar($"Loading {_type}.shn", Color.SteelBlue);
            var watch = Stopwatch.StartNew();

            if (!Load(out var shnResult))
            {
                EngineLog.Error($"SHNLoader->Load() : Failed to load SHN file {_type}");
            }

            progressBar.Update(0);
            for (var i = 0; i < shnResult.Count; i++)
            {
                shnProcFunc.Invoke(shnResult, i);
                
                var raw = (decimal)(0.5f + (100f * i) / shnResult.Count);
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
