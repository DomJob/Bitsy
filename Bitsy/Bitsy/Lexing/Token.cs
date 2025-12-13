using Bitsy.Reading;

namespace Bitsy.Lexing;

public class Token
{
    public Token(TokenType type, Position position, string literal = "")
    {
        Type = type;
        Literal = literal;
        Position = position;
    }

    public TokenType Type { get; }
    public Position Position { get; }
    public string Literal { get; }

    public override string ToString()
    {
        return $"[{Type}, '{Literal}', {Position}]";
    }
}