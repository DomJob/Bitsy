using Bitsy.Reading;

namespace Bitsy.Parsing;

public class SyntaxError : Exception
{
    public SyntaxError(string message, Position position)
    {
        Message = message;
        Position = position;
    }

    public string Message { get; }

    public Position Position { get; }
}