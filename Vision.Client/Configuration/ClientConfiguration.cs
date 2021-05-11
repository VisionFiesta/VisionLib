using Vision.Client.Data;
using Vision.Core.Configuration;

namespace Vision.Client.Configuration
{
    public class ClientConfiguration : Configuration<StaticClientData>
    {
        public ClientConfiguration(string configFolderPath = "") : base(configFolderPath) {}
    }
}
