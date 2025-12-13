namespace Bitsy.Reading;

public interface IReader
{
    bool HasMore();

    char Read();

    char Peek();

    Position GetPosition();
}