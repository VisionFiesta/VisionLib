using System.Collections.Generic;
using Vision.Client.Data;
using Vision.Core.Configuration;

namespace Vision.Client.Configuration
{
    public class UserConfiguration : Configuration<List<ClientUserData>>
    {
        public UserConfiguration(string configFolderPath) : base(configFolderPath) {}

        public void Load()
        {
            if (Load(out var message))
            {
                Logger.Info(message);
            }
            else
            {
                Logger.Warning(message);
                ConfigurationData.Add(new ClientUserData());
            }
        }
    }
}
