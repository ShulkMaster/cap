using System.Linq.Expressions;
using Cap.Contract.Request;
using Cap.Models;

namespace Cap.Utils;

using WhereExp = Expression<Func<Quake, bool>>;

public class QuakeFilterer
{
    private readonly QuakeFilter _filter;

    public QuakeFilterer(QuakeFilter filter)
    {
        _filter = filter;
    }

    public IQueryable<Quake> Apply(IQueryable<Quake> data)
    {
        var result = data;
        if (_filter.Date is not null)
        {
            result = ApplyDateQuery(result, _filter.Date);
        }

        if (_filter.Magnitude is not null)
        {
            var query = RangeQueries.GetMagnitudeQuery(_filter.Magnitude);
            result = ApplyDoubleQuery(result, _filter.Magnitude, query);
        }

        if (_filter.Depth is not null)
        {
            RangeQuery query = RangeQueries.GetDepthQuery(_filter.Depth);
            result = ApplyDoubleQuery(result, _filter.Depth, query);
        }

        if (_filter.Intensity is not null)
        {
            RangeQuery query = RangeQueries.GetIntensityQuery(_filter.Intensity);
            result = ApplySingleQuery(result, _filter.Intensity, query);
        }

        return result;
    }

    public IOrderedQueryable<Quake> Sort(IQueryable<Quake> queryable)
    {
        switch (_filter.Sort)
        {
            case QuakeSort.Date:
                return queryable.OrderBy(q => q.Date);
            case QuakeSort.Latitude:
                return queryable.OrderBy(q => q.Latitude);
            case QuakeSort.Longitude:
                return queryable.OrderBy(q => q.Longitude);
            case QuakeSort.Magnitude:
                return queryable.OrderBy(q => q.Magnitude);
            case QuakeSort.Depth:
                return queryable.OrderBy(q => q.Depth);
            case QuakeSort.Intensity:
                return queryable.OrderBy(q => q.Intensity);
            case QuakeSort.Id:
            default:
                return queryable.OrderBy(q => q.Id);
        }
    }

    private static IQueryable<Quake> ApplyDateQuery(IQueryable<Quake> data, DateRange dates)
    {
        RangeQuery query = RangeQueries.GetDatesQuery(dates);
        (DateTime min, DateTime max) = dates;

        if (min != default && max != default)
        {
            return data.Where(query.OnBoth);
        }

        if (min == default)
        {
            return data.Where(query.OnMax);
        }

        if (max == default)
        {
            return data.Where(query.OnMax);
        }

        return data;
    }

    private static IQueryable<Quake> ApplyDoubleQuery(IQueryable<Quake> data, DoubleRange range, RangeQuery query)
    {
        (double min, double max) = range;

        if (min != 0 && max != 0)
        {
            return data.Where(query.OnBoth);
        }

        if (min == 0)
        {
            return data.Where(query.OnMax);
        }

        if (max == 0)
        {
            return data.Where(query.OnMin);
        }

        return data;
    }
    
    private static IQueryable<Quake> ApplySingleQuery(IQueryable<Quake> data, SingleRange range, RangeQuery query)
    {
        (double min, double max) = range;

        if (min != 0 && max != 0)
        {
            return data.Where(query.OnBoth);
        }

        if (min == 0)
        {
            return data.Where(query.OnMax);
        }

        if (max == 0)
        {
            return data.Where(query.OnMin);
        }

        return data;
    }
}