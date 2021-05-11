using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vision.Core.Logging.Loggers;

namespace Vision.Core.IO.SHN
{
    public class SHNManager
    {
        private readonly EngineLog Logger = new(typeof(SHNManager));

        private readonly string _shnFolder;
        private readonly ISHNCrypto _crypto;
        public SHNManager(string shnFolder, ISHNCrypto crypto)
        {
            _shnFolder = shnFolder;
            _crypto = crypto;
            SimpleSHNLoader.Initialize(shnFolder, crypto);

            if (Directory.Exists(shnFolder)) return;
            Logger.Error("SHNManager->Initialize() : SHN Folder does not exist!");
            throw new Exception("SHNManager->Initialize() : SHN Folder does not exist!");
        }

        public bool Load(SHNType type, out SHNFile file)
        {
            var encoding = type == SHNType.TextData ? Encoding.ASCII : Encoding.GetEncoding("ISO-8859-1");

            file = SHNFile.Create(_shnFolder, type, _crypto, encoding);
            if (file == null) return false;
            file.Read();
            file.DisallowRowChanges();
            return true;
        }

        public SHNLoader GetSHNLoader(SHNType type) => new(_shnFolder, type, _crypto);

        public List<SHNFile> LoadSHNFiles(params SHNType[] types)
        {
            var loadedSHNs = new List<SHNFile>();

            var parallelTypes = types.Where(shnType => shnType.IsLarge());
            var nonParallelTypes = types.Where(shnType => !shnType.IsLarge());

            Parallel.Invoke(new ParallelOptions(),
                () => Parallel.ForEach(parallelTypes,
                            shnType =>
                                    {
                                        if (Load(shnType, out var shnFile))
                                        {
                                            loadedSHNs.Add(shnFile);
                                        }
                                    }),
                        () =>
                        {
                            foreach (var shnType in nonParallelTypes)
                            {
                                if (Load(shnType, out var shnFile))
                                {
                                    loadedSHNs.Add(shnFile);
                                }
                            }
                        });
            return loadedSHNs;
        }

        public void SimpleLoadSHNs(params SHNType[] types) => SimpleSHNLoader.Load(true, types);

        public bool TryGetSimpleSHNObject<T>(SHNType type, out ObjectCollection<T> objects) => SimpleSHNLoader.TryGetObjects(type, out objects);
    }
}
