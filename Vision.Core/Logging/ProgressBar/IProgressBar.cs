using System.Drawing;
using Colorful;
using Console = Colorful.Console;

namespace Vision.Core.Common.Logging.ProgressBar
{
    public abstract class AProgressBar
    {
        private int _barSize = 50;

        public int BarSize
        {
            get => _barSize;
            set
            {
                if (value > 100) value = 100;
                if (value < 10) value = 10;
                _barSize = value;
                EmptyBar = new string(EmptyChar, value);
            }
        }

        // private const string Twirl = "-\\|/";
        protected const char StartChar = '[';
        protected const char EmptyChar = '_';
        protected const char FullChar = '■';
        protected const char EndChar = ']';

        protected string EmptyBar { get; private set; }

        private readonly byte _logLevel;
        private readonly ColorfulConsoleLogger _loggerInstance;
        protected readonly Color _color;

        protected AProgressBar(byte logLevel, ColorfulConsoleLogger loggerInstance, Color? color = null)
        {
            _logLevel = logLevel;
            _loggerInstance = loggerInstance;

            if (!color.HasValue) color = loggerInstance.BaseColor;
            _color = color.Value;
        }

        protected internal void MoveCursor(int cols)
        {
            var curCursorLeft = Console.CursorLeft;
            var newCursorLeft = curCursorLeft - cols - 1;
            if (newCursorLeft < 0) newCursorLeft = 0;

            Console.CursorLeft = newCursorLeft;
        }

        protected internal void Write(params Formatter[] formatters) => _loggerInstance.WriteRawFormatted(_logLevel, formatters);

        protected internal void Write(string str) => _loggerInstance.WriteRaw(_logLevel, str);

        protected internal void WriteLine() => _loggerInstance.WriteLineRaw(_logLevel, "");

        public abstract void Update(int percent);
        public abstract void Complete(string message);
    }
}
