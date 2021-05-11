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
        private static readonly EngineLog Logger = new(typeof(SHNLoader));

        private readonly string _shnFolder;
        private readonly SHNType _type;
        private readonly ISHNCrypto _crypto;

        public string MD5Hash { get; private set; }

        protected internal SHNLoader(string shnFolder, SHNType type, ISHNCrypto crypto)
        {
            _shnFolder = shnFolder;
            _type = type;
            _crypto = crypto;
        }

        public delegate void SHNProcessor(SHNResult result, int index);

        private readonly Queue<Tuple<EngineLogLevel, string>> _messageQueue = new();

        public void QueueMessage(EngineLogLevel level, string message) => _messageQueue.Enqueue(new Tuple<EngineLogLevel, string>(level, message));

        private bool Load(out SHNFile shnFile)
        {
            var encoding = _type == SHNType.TextData ? Encoding.ASCII : Encoding.GetEncoding("ISO-8859-1");

            shnFile = SHNFile.Create(_shnFolder, _type, _crypto, encoding);
            if (shnFile == null) return false;
            shnFile.Read();
            shnFile.DisallowRowChanges();
            MD5Hash = shnFile.MD5Hash;
            return true;
        }

        public void Load(SHNProcessor shnProcFunc)
        {
            var progressBar = Logger.CreateProgressBar($"Loading {_type}.shn", Color.SteelBlue);
            var watch = Stopwatch.StartNew();

            if (!Load(out var shnFile))
            {
                Logger.Error($"SHNLoader->Load() : Failed to load SHN file {_type}");
            }

            var shnResult = shnFile.Data;

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


            _messageQueue.DequeueAll(tuple => { Logger.WriteLine(tuple.Item1, tuple.Item2); });
        }
    }
}
