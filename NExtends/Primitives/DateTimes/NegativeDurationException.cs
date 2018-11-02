using System;

namespace NExtends.Primitives.DateTimes
{
    public class NegativeDurationException : ArgumentException
    {
        public NegativeDurationException(string paramName)
            : base("You cannot create or update a period to a negative duration", paramName) { }
    }
}
