using System.Linq.Expressions;
using Cap.Models;

namespace Cap.Utils;

using WhereExp = Expression<Func<Quake, bool>>;

public class RangeQuery
{
    private readonly WhereExp _onDefault = (quake) => true;
    public WhereExp OnBoth { get; private set; }
    public WhereExp OnMin { get; private set; }
    public WhereExp OnMax { get; private set; }

    public RangeQuery()
    {
        OnBoth = _onDefault;
        OnMin = _onDefault;
        OnMax = _onDefault;
    }

    public RangeQuery AddOnBothRanges(WhereExp exp)
    {
        OnBoth = exp;
        return this;
    }

    public RangeQuery AddOnMinRange(WhereExp exp)
    {
        OnMin = exp;
        return this;
    }

    public RangeQuery AddOnMaxRange(WhereExp exp)
    {
        OnMax = exp;
        return this;
    }
}