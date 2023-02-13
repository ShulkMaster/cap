using Cap.Contract.Request;

namespace Cap.Utils;

public static class RangeQueries
{
    public static RangeQuery GetMagnitudeQuery(DoubleRange magnitudes)
    {
        (double min, double max) = magnitudes;
        RangeQuery query = new RangeQuery()
            .AddOnMaxRange(quake => quake.Magnitude <= max)
            .AddOnMinRange(quake => quake.Magnitude >= min)
            .AddOnBothRanges(quake => quake.Magnitude <= max && quake.Magnitude >= min);
        return query;
    }

    public static RangeQuery GetDatesQuery(DateRange dates)
    {
        RangeQuery query = new RangeQuery()
            .AddOnMaxRange(quake => quake.Date <= dates.Max)
            .AddOnMinRange(quake => quake.Date >= dates.Min)
            .AddOnBothRanges(quake => quake.Date <= dates.Max && quake.Date >= dates.Min);
        return query;
    }
    
    public static RangeQuery GetDepthQuery(DoubleRange depths)
    {
        (double min, double max) = depths;
        RangeQuery query = new RangeQuery()
            .AddOnMaxRange(quake => quake.Depth <= max)
            .AddOnMinRange(quake => quake.Depth >= min)
            .AddOnBothRanges(quake => quake.Depth <= max && quake.Depth >= min);
        return query;
    }
}