using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Vision.Core.Common.Collections;
using Vision.Core.Common.Logging.Loggers;

namespace Vision.Core.Common.IO.SHN
{
    public class SHNLoader<TKey, TValue>
    {
        private readonly SHNType _type;

        public SHNLoader(SHNType type)
        {
            _type = type;
        }

        public delegate void SHNProcessor(SHNResult result, int index);

        public delegate KeyValuePair<TKey, TValue> SHNDictProcessor(SHNResult result, int index);

        private readonly FastConcurrentDictionary<TKey, TValue> _outputDict = new FastConcurrentDictionary<TKey, TValue>();

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
            progressBar.Complete(cmpMessage);
        }

        public IDictionary<TKey, TValue> Load(SHNDictProcessor shnProcFunc, bool reload = false)
        {
            if (!_outputDict.IsEmpty && !reload) return _outputDict;

            var progressBar = EngineLog.CreateProgressBar($"Loading {_type}.shn", Color.SteelBlue);
            var watch = Stopwatch.StartNew();

            SHNManager.Load(_type, out var shnResult);

            progressBar.Update(0);
            for (var i = 0; i < shnResult.Count; i++)
            {
                var result = shnProcFunc.Invoke(shnResult, i);
                _outputDict.Add(result.Key, result.Value);

                var raw = (decimal)(0.5f + ((100f * i) / shnResult.Count));
                var percent = Math.Round(raw, MidpointRounding.ToEven);

                progressBar.Update((int)percent);
            }
            watch.Stop();
            var cmpMessage =
                $"Loaded {_outputDict.Count()} entries from {_type}.shn in {watch.Elapsed.TotalMilliseconds}ms";
            progressBar.Complete(cmpMessage);

            return _outputDict;
        }
    }
}
