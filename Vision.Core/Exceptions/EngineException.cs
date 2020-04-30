using System;

namespace Vision.Core.Common.Exceptions
{
    public class EngineException : Exception
    {
        public EngineException(string msg) : base(msg) { }
    }
}
