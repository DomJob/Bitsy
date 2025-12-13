namespace Bitsy.Reading;

public class Reader
{
    private readonly Stack<Position> positions = new();

    public Position GetPosition()
    {
        return positions.Peek();
    }
}