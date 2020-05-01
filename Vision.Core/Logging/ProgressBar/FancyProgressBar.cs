using System.Collections.Generic;
using System.Drawing;
using Colorful;
using Vision.Core.Extensions;
using Console = Colorful.Console;

namespace Vision.Core.Logging.ProgressBar
{
    public class FancyProgressBar : AProgressBar
    {
        private readonly Formatter[] _prefix;
        private readonly int _prePercentLen;

        private int _lastPercent;
        private bool _hasWrittenOnce;
        private int FullLen => _prePercentLen + _lastPercent.Digits();

        public FancyProgressBar(List<Formatter> prefix, byte logLevel, ColorfulConsoleLogger loggerInstance, Color? color = null) : base(logLevel, loggerInstance)
        {
            _prefix = prefix.ToArray();
            _prePercentLen = prefix.Length() + 5 + BarSize;
        }

        public override void Complete(string message, Color color)
        {
            // MoveCursor(FullLen - _prefixLength - 1);
            Update(100);
            Write(" -> " + message, color);
            WriteLine();
        }

        public override void Update(int percent)
        {
            // no need to run if nothing has changed
            var isFirstRun = percent == 0 && !_hasWrittenOnce;
            if (percent <= _lastPercent && !isFirstRun) return;

            Console.ForegroundColor = Color;

            // clear
            if (!_hasWrittenOnce)
            {
                Write(_prefix);
                var firstWriteStr = StartChar + EmptyBar + EndChar + "  0%";
                Write(firstWriteStr);
                _hasWrittenOnce = true;
                Console.ResetColor();
                return;
            }

            // go to start of line and write prefix
            MoveCursor(FullLen);
            Write(_prefix);

            var chunk = BarSize / (100f / percent);

            var p = (percent / (float)10 + 0.5f) * BarSize / 10f - 0.5f;
            var fillStr = "";

            for (var i = 0; i < BarSize; ++i)
            {
                fillStr += i >= chunk ? EmptyChar : FullChar;
            }

            var str = StartChar + fillStr + EndChar + $"{percent,3:##0}%";
            Write(str);

            Console.ResetColor();

            _lastPercent = percent;
        }
    }
}
