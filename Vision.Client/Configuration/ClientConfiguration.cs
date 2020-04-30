using Vision.Client.Data;
using Vision.Core.Configuration;

namespace Vision.Client.Configuration
{
    public class ClientConfiguration : Configuration<ClientConfiguration>
    {
        public ClientData Data = new ClientData();
    }
}
