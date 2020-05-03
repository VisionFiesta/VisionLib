using System.IO;
using System.Text;
using Vision.Core.Logging.Loggers;

namespace Vision.Core.IO.SHN
{
    public class SHNManager
    {
        private static SHNManager _instance;

        private readonly string _shnFolder;
        private readonly ISHNCrypto _crypto;

        private SHNManager(string shnFolder, ISHNCrypto crypto)
        {
            _shnFolder = shnFolder;
            _crypto = crypto;
            SimpleSHNLoader.Initialize(shnFolder, crypto);
        }

        public static bool Initialize(string shnFolder, ISHNCrypto crypto)
        {
            if (!Directory.Exists(shnFolder))
            {
                EngineLog.Error("SHNManager->Initialize() : SHN Folder does not exist!");
                return false;
            }

            _instance = new SHNManager(shnFolder, crypto);
            return true;
        }

        protected internal static bool Load(SHNType type, out SHNResult result)
        {
            result = new SHNResult();

            if (_instance == null)
            {
                EngineLog.Error("SHNManager->Load() : Attempted to call SHNManager uninitialized!");
                return false;
            }

            var encoding = type == SHNType.TextData ? Encoding.ASCII : Encoding.GetEncoding("ISO-8859-1");

            var shnFile = SHNFile.Create(_instance._shnFolder, type, _instance._crypto, encoding);
            if (shnFile == null) return false;
            shnFile.Read();
            shnFile.DisallowRowChanges();

            result.Load(shnFile);
            return true;
        }

        public static SHNLoader GetSHNLoader(SHNType type) => new SHNLoader(_instance._shnFolder, type, _instance._crypto);

        public static void SimpleLoadSHNs(params SHNType[] types) => SimpleSHNLoader.Load(true, types);

        public static bool TryGetSimpleSHNObject<T>(SHNType type, out ObjectCollection<T> objects) => SimpleSHNLoader.TryGetObjects(type, out objects);
    }
}
