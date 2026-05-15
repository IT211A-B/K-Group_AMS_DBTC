namespace Backend.Backend.Helper
{
    public static class StringToDayOfWeek
    {
        public static DayOfWeek ConvertFromStringToDayOfWeek(this string day)
        {
            DayOfWeek result = DayOfWeek.Sunday;
            switch (day)
            {
                case "Monday":
                    result = DayOfWeek.Monday;
                    break;
                case "Tuesday":
                    result = DayOfWeek.Tuesday;
                    break;
                case "Wednesday":
                    result = DayOfWeek.Wednesday;
                    break;
                case "Thursday":
                    result = DayOfWeek.Thursday;
                    break;
                case "Friday":
                    result = DayOfWeek.Friday;
                    break;
                case "Saturday":
                    result = DayOfWeek.Saturday;
                    break;
            }
            return result;
        }
    }
}
