using System;
using System.Diagnostics;

namespace VisionLib.Client
{
    public class ClientGameTime
    {
        public DateTime StartTime { get; private set; }
        private readonly Stopwatch _watch = new Stopwatch();

        public ClientGameTime()
        {
            Set(DateTime.Now);
        }

        public ClientGameTime(DateTime startTime)
        {
           Set(startTime);
        }

        public void Set(DateTime startTime)
        {
            StartTime = startTime;
            _watch.Reset();
            _watch.Start();
        }

        public DateTime Now => StartTime.Add(_watch.Elapsed);
    }
}
