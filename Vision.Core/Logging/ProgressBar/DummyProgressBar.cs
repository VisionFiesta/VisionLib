using System.Drawing;

namespace Vision.Core.Logging.ProgressBar
{
    public class DummyProgressBar : AProgressBar
    {
        private readonly byte _logLevel;
        private readonly ColorfulConsoleLogger _loggerInstance;

        public DummyProgressBar(byte logLevel, ColorfulConsoleLogger loggerInstance) : base(logLevel, loggerInstance)
        {
            _logLevel = logLevel;
            _loggerInstance = loggerInstance;
        }

        public override void Update(int percent)
        {
            // nothin
        }

        public override void Complete(string message, Color color)
        {
            _loggerInstance.WriteLine(_logLevel, "Info", message, color);
        }
    }
}
