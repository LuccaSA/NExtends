using System.Globalization;

namespace NExtends.Primitives.TimeSpans
{
    public class TimeInitials
    {
        private static readonly TimeInitials _french = new TimeInitials("m", "h", "j");
        private static readonly TimeInitials _german = new TimeInitials("M", "St", "T");
        private static readonly TimeInitials _english = new TimeInitials("m", "h", "d");

        public string MinutesInitial { get; }
        public string HoursInitial { get; }
        public string DaysInitial { get; }

        public TimeInitials(string minutesInitial, string hoursInitial, string daysInitial)
        {
            MinutesInitial = minutesInitial;
            HoursInitial = hoursInitial;
            DaysInitial = daysInitial;
        }

        public static TimeInitials FromCulture(CultureInfo culture)
        {
            switch(culture.Name)
            {
                case "fr-FR":
                    return _french;
                case "de-DE":
                    return _german;
                default:
                    return _english;
            }
        }
    }
}
