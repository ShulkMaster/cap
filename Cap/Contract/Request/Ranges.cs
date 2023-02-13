namespace Cap.Contract.Request;

public class DoubleRange
{
    public const double Tolerance = 0.0005d;
    public double Min { get; set; }
    public double Max { get; set; }
}

public class SingleRange
{
    public const double Tolerance = 0.0005d;
    public float Min { get; set; }
    public float Max { get; set; }
}

public class DateRange
{
    public DateTime Min { get; set; }
    public DateTime Max { get; set; }
}