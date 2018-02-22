using System;

namespace NExtends.Time
{
    public interface ITimeService
    {
        DateTime Today { get; }
        DateTime Now { get; }
    }
}
