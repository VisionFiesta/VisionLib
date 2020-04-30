namespace Vision.Game.Enums
{
    public enum WorldServerStatus : byte
    {
        // With messages:
        Closed = 0,        // Server is closed. Please try login later.
        Maintenance = 1,    // Server is under maintenance. Please try login later.
        Offline = 2,    // You cannot connect to an empty server.
        Reserved = 3,       // The server has been reserved for a special use.
        Unknown = 4, // Login failed due to an unknown error.
        Full = 5,           // Server is full. Please try again later.
        Low = 6,
        Low2 = 7,
        Low3 = 8,
        Medium = 9,
        High = 10,
    }

    public static class WorldServerStatusExtensions
    {
        public static bool IsJoinable(this WorldServerStatus status)
        {
            switch (status)
            {
                case WorldServerStatus.Closed:
                case WorldServerStatus.Maintenance:
                case WorldServerStatus.Offline:
                case WorldServerStatus.Reserved:
                case WorldServerStatus.Unknown:
                case WorldServerStatus.Full:
                    return false;
                case WorldServerStatus.Low:
                case WorldServerStatus.Low2:
                case WorldServerStatus.Low3:
                case WorldServerStatus.Medium:
                case WorldServerStatus.High:
                    return true;
                default:
                    return false; ;
            }
        }

        public static string ToMessage(this WorldServerStatus status)
        {
            switch (status)
            {
                case WorldServerStatus.Closed:
                    return "Server is closed. Please try login later.";
                case WorldServerStatus.Maintenance:
                    return "Server is under maintenance. Please try login later.";
                case WorldServerStatus.Offline:
                    return "You cannot connect to an empty server.";
                case WorldServerStatus.Reserved:
                    return "The server has been reserved for a special use.";
                case WorldServerStatus.Unknown:
                    return "Login failed due to an unknown error.";
                case WorldServerStatus.Full:
                    return "Server is full. Please try again later.";
                case WorldServerStatus.Low:
                case WorldServerStatus.Low2:
                case WorldServerStatus.Low3:
                    return "Low";
                case WorldServerStatus.Medium:
                    return "Medium";
                case WorldServerStatus.High:
                    return "High";
                default:
                    return "Unknown server state.";
            }
        }
    }
}