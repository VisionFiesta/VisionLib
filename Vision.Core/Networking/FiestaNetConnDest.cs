namespace Vision.Core.Networking
{
    public enum FiestaNetConnDest
    {
        FNCDEST_LOGIN,
        FNCDEST_WORLDMANAGER,
        FNCDEST_ZONE,
        FNCDEST_CLIENT
    }

    public static class FiestaNetworkConnectionDestinationExtensions
    {
        public static bool IsServer(this FiestaNetConnDest dest)
        {
            return dest != FiestaNetConnDest.FNCDEST_CLIENT;
        }

        public static bool IsClient(this FiestaNetConnDest dest)
        {
            return dest == FiestaNetConnDest.FNCDEST_CLIENT;
        }

        public static string ToMessage(this FiestaNetConnDest dest)
        {
            switch (dest)
            {
                case FiestaNetConnDest.FNCDEST_LOGIN:
                    return "Login";
                case FiestaNetConnDest.FNCDEST_WORLDMANAGER:
                    return "WorldManager";
                case FiestaNetConnDest.FNCDEST_ZONE:
                    return "Zone";
                case FiestaNetConnDest.FNCDEST_CLIENT:
                    return "Client";
                default:
                    return "Unknown";
            }
        }
    }
}
