namespace Cap.Models;

public class Quake
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Location { get; set; } = string.Empty;
    public double Depth { get; set; }
    public double Magnitude { get; set; }
    public float Intensity { get; set; }
    public string IntensityDescription { get; set; } = string.Empty;
    public byte[] Geom { get; set; } = Array.Empty<byte>();
}