using Bitsy.Reading;

namespace Bitsy.Lexing;

public class Token
{
    public static Token Main = new(TokenType.Identifier, new Position(), "main");
    public static Token Zero = new(TokenType.Identifier, new Position(), "0");
    public static Token One = new(TokenType.Identifier, new Position(), "1");

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