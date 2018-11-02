using System;

namespace NExtends.Primitives.DateTimes
{
    public struct Period : ITimeBlock
    {
        public DateTime StartsAt { get; }
        public DateTime EndsAt { get; }
        public TimeSpan Duration { get; }

        public Date StartsOn { get; }
        public Date EndsOn { get; }

        [Obsolete("You should use StartsAt or StartsOn instead")]
        public DateTime Start => StartsAt;
        [Obsolete("You should use EndsAt or EndsOn instead")]
        public DateTime End => EndsAt;

        public Period(DateTime startsAt, DateTime endsAt)
            : this(startsAt, endsAt, endsAt - startsAt) { }

        public Period(DateTime startsAt, DateTime endsAt, TimeSpan duration)
        {
            if (duration < TimeSpan.Zero)
            {
                throw new NegativeDurationException(nameof(duration));
            }

            StartsAt = startsAt;
            EndsAt = endsAt;
            Duration = duration;

            StartsOn = new Date(startsAt);
            EndsOn = new Date(startsAt);
        }

        public Period(Date startsOn, Date endsOn)
        {
            StartsOn = startsOn;
            EndsOn = endsOn;
            StartsAt = new DateTime(startsOn.Ticks);
            EndsAt = new DateTime(endsOn.Ticks);
            Duration = endsOn - startsOn;

            if (Duration < TimeSpan.Zero)
            {
                throw new NegativeDurationException(nameof(endsOn));
            }
        }

        ITimeBlock ITimeBlock.ChangeStartsAt(DateTime startsAt)
        {
            if (startsAt > EndsAt)
            {
                throw new NegativeDurationException(nameof(startsAt));
            }

            return new Period(startsAt, EndsAt);
        }
        ITimeBlock ITimeBlock.ChangeEndsAt(DateTime endsAt)
        {
            if (endsAt < StartsAt)
            {
                throw new NegativeDurationException(nameof(endsAt));
            }

            return new Period(StartsAt, endsAt);
        }
        ITimeBlock ITimeBlock.ChangeDuration(TimeSpan duration)
        {
            if (duration < TimeSpan.Zero)
            {
                throw new NegativeDurationException(nameof(duration));
            }

            return new Period(StartsAt, EndsAt, duration);
        }
    }
}
