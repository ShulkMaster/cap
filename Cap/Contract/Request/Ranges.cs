namespace Cap.Contract.Request;

public class DoubleRange
{
    public double Min { get; set; }
    public double Max { get; set; }

    public void Deconstruct(out double min, out double max)
    {
        min = Min;
        max = Max;
    }
}

public class SingleRange
{
    public float Min { get; set; }
    public float Max { get; set; }
    
    public void Deconstruct(out float min, out float max)
    {
        min = Min;
        max = Max;
    }
}

public class DateRange
{
    public DateTime Min { get; set; }
    public DateTime Max { get; set; }

    public void Deconstruct(out DateTime min, out DateTime max)
    {
        min = Min;
        max = Max;
    }
}