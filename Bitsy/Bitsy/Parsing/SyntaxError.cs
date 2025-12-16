using Bitsy.Reading;

namespace Bitsy.Parsing;

public class SyntaxError : Exception
{
    public string Message { get; }
    
    public Position Position { get; }

    public SyntaxError(string message, Position position)
    {
        Message = message;
        Position = position;
    }
}