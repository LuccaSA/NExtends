using System;

namespace NExtends.Primitives.DateTimes
{
    public class Period : ITimeBlock, IEquatable<Period>
    {
        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }
        public TimeSpan Duration { get; private set; }

        public DateTime StartsAt => Start;
        public DateTime EndsAt => End;

        void ITimeBlock.ChangeStartsAt(DateTime startsAt)
        {
            if (startsAt > End)
            {
                throw new NegativeDurationException(nameof(startsAt));
            }

            Start = startsAt;
        }
        void ITimeBlock.ChangeEndsAt(DateTime endsAt)
        {
            if (endsAt < Start)
            {
                throw new NegativeDurationException(nameof(endsAt));
            }

            End = endsAt;
        }
        void ITimeBlock.ChangeDuration(TimeSpan duration)
        {
            if (duration < TimeSpan.Zero)
            {
                throw new NegativeDurationException(nameof(duration));
            }

            Duration = duration;
        }

        public Period(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
            Duration = end - start;

            if (Duration < TimeSpan.Zero)
            {
                throw new NegativeDurationException(nameof(end));
            }
        }

        public bool Equals(Period other)
        {
            if (other == null) { return false; }

            return Start == other.Start &&
                End == other.End &&
                Duration == other.Duration;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            if (obj.GetType() != GetType()) { return false; }

            return Equals(obj as Period);
        }
        public override int GetHashCode()
        {
            //cf http://www.aaronstannard.com/overriding-equality-in-dotnet/
            unchecked
            {
                var hashCode = 13;
                hashCode = (hashCode * 397) ^ Start.GetHashCode();
                hashCode = (hashCode * 397) ^ End.GetHashCode();
                hashCode = (hashCode * 397) ^ Duration.GetHashCode();
                return hashCode;
            }
        }
    }
}
