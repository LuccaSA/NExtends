using System;

namespace NExtends.Time
{
    public class WaybackTimeService : ITimeService
    {
        private readonly DateTime _reference;
        private readonly DateTime _instantiatedAt;

        public WaybackTimeService(DateTime reference)
        {
            _reference = reference;
            _instantiatedAt = DateTime.Now;
        }

        public DateTime Today => Now.Date;
        public DateTime Now => _reference.AddTicks((DateTime.Now - _instantiatedAt).Ticks);
    }
}
