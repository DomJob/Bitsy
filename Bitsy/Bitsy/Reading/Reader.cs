namespace Bitsy.Reading;

public class Reader
{
    private Stack<Position> positions = new Stack<Position>();

    public Position GetPosition() => positions.Peek();
}