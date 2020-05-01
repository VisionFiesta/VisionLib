using System;

namespace Vision.Core.Exceptions
{
    public class EngineException : Exception
    {
        public EngineException(string msg) : base(msg) { }
    }
}
