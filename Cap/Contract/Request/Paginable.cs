namespace Cap.Contract.Request;

public class Paginable
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 20;
}