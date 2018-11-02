using System;

namespace NExtends.Primitives.DateTimes
{
    public interface ITimeBlock
    {
        DateTime StartsAt { get; }
        DateTime EndsAt { get; }
        TimeSpan Duration { get; }

        void ChangeStartsAt(DateTime startsAt);
        void ChangeEndsAt(DateTime endsAt);
        void ChangeDuration(TimeSpan duration);
    }
}
