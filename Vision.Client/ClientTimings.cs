using System;

namespace Vision.Client
{
    public static class ClientTimings
    {
        public static readonly TimeSpan WorldListUpdatePeriod = TimeSpan.FromSeconds(10);

        public static readonly TimeSpan GameTimeUpdatePeriod = TimeSpan.FromMinutes(1);

    }
}
