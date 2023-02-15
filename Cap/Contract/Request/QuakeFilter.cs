namespace Cap.Contract.Request;

public class QuakeFilter: Paginable
{
    public QuakeSort Sort { get; set; } = QuakeSort.Magnitude;
    public DoubleRange? Magnitude { get; set; }
    public DoubleRange? Depth { get; set; }
    public SingleRange? Intensity { get; set; }
    public DateRange? Date { get; set; }
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