using System;

namespace NExtends.Primitives.DateTimes
{
    public struct Period
    {
        public DateTime Start { get; }
        public DateTime End { get; }
        public TimeSpan Duration { get { return End - Start; } }

        public Period(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }
    }
}
