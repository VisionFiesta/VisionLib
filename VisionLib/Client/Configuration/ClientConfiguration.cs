using VisionLib.Client.Data;
using VisionLib.Common.Configuration;

namespace VisionLib.Client.Configuration
{
    public class ClientConfiguration : Configuration<ClientConfiguration>
    {
        public ClientData Data { get; protected set; } = new ClientData();
    }
}
