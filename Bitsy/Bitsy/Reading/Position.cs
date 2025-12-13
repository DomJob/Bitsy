namespace Bitsy.Reading;

public class Position
{
    public String FileName;
    public int Line;
    public int Column;
    
    public override string ToString() => $"in {FileName} - {Line}:{Column}";
}