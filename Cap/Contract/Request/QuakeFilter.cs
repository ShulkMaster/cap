namespace Cap.Contract.Request;

public class QuakeFilter: Paginable
{
    public QuakeSort Sort { get; set; } = QuakeSort.Id;
    public DoubleRange Magnitude { get; set; }
    public DoubleRange Depth { get; set; }
    public SingleRange Intensity { get; set; }
    public DoubleRange Latitude { get; set; }
    public DoubleRange Longitude { get; set; }
    public string DescLike { get; set; } = string.Empty;
    public DateRange? Date { get; set; }
    public DateTime Xd { get; set; }
}

public enum QuakeSort
{
    Id,
    Date,
    Latitude,
    Longitude,
    Magnitude,
    Depth,
    Intensity,
}