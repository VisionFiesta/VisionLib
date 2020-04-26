using VisionLib.Client.Data;
using VisionLib.Client.Enums;
using VisionLib.Common.Networking;

namespace VisionLib.Client.Services
{
    public class ClientZoneService
    {
        public ClientZoneServiceStatus ZoneStatus { get; private set; } = ClientZoneServiceStatus.CZSS_NOTCONNECTED;

        private readonly FiestaClient _client;
        private FiestaNetConnection ZoneConnection => _client.ZoneClient;
        private ClientUserData Config => _client.UserData;
        private ClientWorldService WorldService => _client.WorldService;

        private readonly ClientWorldServiceData _data = new ClientWorldServiceData();

        public ClientZoneService(FiestaClient client)
        {
            _client = client;
        }

        public void SetStatus(ClientZoneServiceStatus status)
        {
            ZoneStatus = status;
            Update();
        }

        private void Update()
        {
            switch (ZoneStatus)
            {
                case ClientZoneServiceStatus.CZSS_NOTCONNECTED:
                {
                    break;
                }
                case ClientZoneServiceStatus.CZSS_TRYCONNECT:
                {
                    break;
                }
                case ClientZoneServiceStatus.CZSS_CONNECTED:
                {
                    break;
                }
            }
        }
    }
}
