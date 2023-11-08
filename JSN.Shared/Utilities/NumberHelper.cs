namespace JSN.Shared.Utilities;

public static class NumberHelper
{
    public static decimal RoundDecimal(decimal? price, int numDecPlace = 2)
    {
        if (!price.HasValue)
        {
            return 0;
        }

        if (price % 1 == 0)
        {
            return (decimal)price;
        }

        return Math.Round(price.Value, numDecPlace, MidpointRounding.AwayFromZero);
    }

    public static double RoundDouble(double quantity, int numDecPlace = 2)
    {
        return Math.Round(quantity, numDecPlace, MidpointRounding.AwayFromZero);
    }
}
