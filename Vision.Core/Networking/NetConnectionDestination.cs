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
        public static bool IsServer(this NetConnectionDestination dest) => dest != NetConnectionDestination.NCD_CLIENT;

        public static bool IsClient(this NetConnectionDestination dest) => dest == NetConnectionDestination.NCD_CLIENT;

        public static string ToMessage(this NetConnectionDestination dest)
        {
            return dest switch
            {
                NetConnectionDestination.NCD_LOGIN => "Login",
                NetConnectionDestination.NCD_WORLDMANAGER => "WorldManager",
                NetConnectionDestination.NCD_ZONE => "Zone",
                NetConnectionDestination.NCD_CLIENT => "Client",
                _ => "Unknown"
            };
        }
    }
}
