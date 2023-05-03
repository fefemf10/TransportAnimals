namespace TransportAnimals.Helpers
{
    public static class DateTimeOffsetTrunc
    {
        public static DateTimeOffset Truncate(DateTimeOffset dateTime)
        {
            return new DateTimeOffset(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, TimeSpan.Zero);
        }
    }
}
