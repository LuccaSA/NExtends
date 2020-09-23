using System;

namespace NExtends.Primitives.DateTimes
{
    public interface ITimeBlock
    {
        DateTime StartsAt { get; }
        DateTime EndsAt { get; }
        TimeSpan Duration { get; }

        ITimeBlock ChangeStartsAt(DateTime startsAt);
        ITimeBlock ChangeEndsAt(DateTime endsAt);
        ITimeBlock ChangeDuration(TimeSpan duration);
    }
}
