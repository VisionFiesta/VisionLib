using System.Drawing;

namespace Vision.Core.Logging.ProgressBar
{
    public class DummyProgressBar : AProgressBar
    {
        public DummyProgressBar(byte logLevel, ColorfulConsoleLogger loggerInstance) : base(logLevel, loggerInstance)
        {
        }

        public override void Update(int percent)
        {
            // nothin
        }

        public override void Complete(string message, Color color)
        {
            Write(message, color);
            WriteLine();
        }
    }
}
