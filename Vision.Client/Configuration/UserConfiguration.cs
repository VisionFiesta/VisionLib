using Vision.Client.Data;
using Vision.Core.Configuration;

namespace Vision.Client.Configuration
{
    public class UserConfiguration : Configuration<UserConfiguration>
    {
        public ClientUserData Data { get; protected set; } = new ClientUserData();
    }
}
