using System.IO;
using System.Text;
using Vision.Core.Common.Logging.Loggers;
using Vision.Core.IO.SHN;

namespace Vision.Core.Common.IO.SHN
{
    public class SHNManager
    {
        private static SHNManager _instance;

        private readonly string _shnPath;
        private readonly ISHNCrypto _crypto;

        private SHNManager(string shnPath, ISHNCrypto crypto)
        {
            _shnPath = shnPath;
            _crypto = crypto;
        }

        public static bool Initialize(string shnPath, ISHNCrypto crypto)
        {
            if (!Directory.Exists(shnPath))
            {
                EngineLog.Error("SHNManager: SHN Folder does not exist!");
                return false;
            }

            _instance = new SHNManager(shnPath, crypto);
            return true;
        }

        public static bool Load(SHNType type, out SHNResult result)
        {
            result = new SHNResult();

            if (_instance == null)
            {
                EngineLog.Error("Attempted to call SHNManager uninitialized!");
                return false;
            }

            var encoding = type == SHNType.TextData ? Encoding.ASCII : Encoding.GetEncoding("ISO-8859-1");

            var shnFile = SHNFile.Create(_instance._shnPath, type, _instance._crypto, encoding);
            if (shnFile == null) return false;
            shnFile.Read();
            shnFile.DisallowRowChanges();

            result.Load(shnFile);
            return true;
        }
    }
}
