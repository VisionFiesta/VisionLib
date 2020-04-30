namespace Vision.Core.Networking
{
    public enum NetConnectionDestination
    {
        NCD_LOGIN,
        NCD_WORLDMANAGER,
        NCD_ZONE,
        NCD_CLIENT
    }

    public static class NetConnectionDestinationExtensions
    {
        public static bool IsServer(this NetConnectionDestination dest)
        {
            return dest != NetConnectionDestination.NCD_CLIENT;
        }

        public static bool IsClient(this NetConnectionDestination dest)
        {
            return dest == NetConnectionDestination.NCD_CLIENT;
        }

        public static string ToMessage(this NetConnectionDestination dest)
        {
            switch (dest)
            {
                case NetConnectionDestination.NCD_LOGIN:
                    return "Login";
                case NetConnectionDestination.NCD_WORLDMANAGER:
                    return "WorldManager";
                case NetConnectionDestination.NCD_ZONE:
                    return "Zone";
                case NetConnectionDestination.NCD_CLIENT:
                    return "Client";
                default:
                    return "Unknown";
            }
        }
    }
}
