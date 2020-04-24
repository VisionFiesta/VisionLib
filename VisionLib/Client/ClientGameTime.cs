using System;
using System.Diagnostics;

namespace VisionLib.Client
{
    public class ClientGameTime
    {
        public readonly DateTime StartTime;
        private readonly Stopwatch Watch = new Stopwatch();

        public ClientGameTime(DateTime startTime)
        {
            StartTime = startTime;
            Watch.Reset();
            Watch.Start();
        }

        public DateTime Now => StartTime.Add(Watch.Elapsed);
    }
}
