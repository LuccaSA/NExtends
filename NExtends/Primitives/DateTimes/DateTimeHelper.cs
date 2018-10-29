using System;

namespace NExtends.Primitives.DateTimes
{
    public class DateTimeHelper
    {
        public static DateTime GetDateOfFirstDayOfWeek(int year, int week)
        {
            for (DateTime date = new DateTime(year, 1, 1).AddDays((week - 1) * 7); date.Year == year; date = date.AddDays(1))
            {
                int temporaryWeek = date.GetISO8601WeekNumber();
                if (temporaryWeek == week)
                {
                    return date;
                }
            }
            return new DateTime(year, 1, 1);
        }
    }
}
