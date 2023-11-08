namespace JSN.Shared.Utilities;

public static class DateTimeHelper
{
    public static double DifferenceBySeconds(DateTime start, DateTime end)
    {
        return NumberHelper.RoundDouble(end.Subtract(start).TotalSeconds);
    }
}