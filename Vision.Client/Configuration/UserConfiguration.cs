using System.Collections.Generic;
using Vision.Client.Data;
using Vision.Core.Configuration;

namespace Vision.Client.Configuration
{
    public class UserConfiguration : Configuration<List<ClientUserData>>
    {
        public UserConfiguration(string configFolderPath) : base(configFolderPath) { }

        protected override List<ClientUserData> GetDataDefault() => new() { new ClientUserData() };
    }
}