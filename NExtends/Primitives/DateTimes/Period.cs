using System;

namespace NExtends.Primitives.DateTimes
{
    public struct Period
    {
        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }

        public Period(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }
    }
}
