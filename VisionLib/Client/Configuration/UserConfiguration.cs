using VisionLib.Client.Data;
using VisionLib.Common.Configuration;

namespace VisionLib.Client.Configuration
{
    public class UserConfiguration : Configuration<UserConfiguration>
    {
        public ClientUserData Data { get; protected set; } = new ClientUserData();
    }
}
