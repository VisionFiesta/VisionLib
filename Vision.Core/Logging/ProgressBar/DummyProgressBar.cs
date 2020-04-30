namespace Vision.Core.Common.Logging.ProgressBar
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

        public override void Complete(string message)
        {
            Write(message);
            WriteLine();
        }
    }
}
