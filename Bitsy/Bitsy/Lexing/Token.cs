using Bitsy.Reading;

namespace Bitsy.Lexing;

public class Token
{
    public TokenType Type { get; private set; }
    public Position Position { get; private set; }
    public string Literal { get; private set; }

    public Token(TokenType type, Position position, string literal = "")
    {
        Type = type;
        Literal = literal;
        Position = position;
    }
}