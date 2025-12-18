namespace Bitsy.Reading;

public class Position
{
    public int Column;
    public string FileName;
    public int Line;

    public override string ToString()
    {
        return $"{FileName} @ {Line}:{Column}";
    }
}