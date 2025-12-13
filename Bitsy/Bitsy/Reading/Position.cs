namespace Bitsy.Reading;

public class Position
{
    public int Column;
    public string FileName;
    public int Line;

    public override string ToString()
    {
        return $"in {FileName} - {Line}:{Column}";
    }
}