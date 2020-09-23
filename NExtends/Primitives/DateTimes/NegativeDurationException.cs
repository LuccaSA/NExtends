using System;
using System.Runtime.Serialization;

namespace NExtends.Primitives.DateTimes
{
    [Serializable]
    public class NegativeDurationException : ArgumentException
    {
        protected NegativeDurationException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public NegativeDurationException(string paramName)
            : base("You cannot create or update a period to a negative duration", paramName) { }
    }
}
