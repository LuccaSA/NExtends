using System;

namespace NExtends.Time
{
    public class StandardTimeService : ITimeService
    {
        public DateTime Today => DateTime.Today;
        public DateTime Now => DateTime.Now;
    }
}
