using System.Collections.Generic;
using Vision.Client.Data;
using Vision.Core.Configuration;

namespace Vision.Client.Configuration
{
    public class UserConfiguration : Configuration<UserConfiguration>
    {
        public UserConfiguration()
        {
            Data.Add(new ClientUserData());
        }

        public List<ClientUserData> Data { get; protected set; } = new List<ClientUserData>();
    }
}
