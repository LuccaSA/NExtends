namespace NExtends.Primitives.TimeSpans
{
    public class TimeInitials
    {
        public string MinutesInitial { get; }
        public string HoursInitial { get; }
        public string DaysInitial { get; }

        public TimeInitials(string minutesInitial, string hoursInitial, string daysInitial)
        {
            MinutesInitial = minutesInitial;
            HoursInitial = hoursInitial;
            DaysInitial = daysInitial;
        }
    }
}
