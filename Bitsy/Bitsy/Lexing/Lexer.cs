using Bitsy.Reading;

namespace Bitsy.Lexing;

public class Lexer
{
    private readonly Queue<Token> peeked = new();
    private readonly IReader reader;

    public Lexer(IReader reader)
    {
        this.reader = reader;
        while (peeked.Count < 5)
            peeked.Enqueue(Consume());
    }

    public Token Next()
    {
        var token = peeked.Dequeue();
        peeked.Enqueue(Consume());

        return token;
    }

    private Token Consume()
    {
        if (!reader.HasMore()) return new Token(TokenType.End, reader.GetPosition());

        var literal = reader.Read();

        var type = TokenType.Illegal;

        switch (literal)
        {
            case '{': type = TokenType.LeftBrace; break;
            case '}': type = TokenType.RightBrace; break;
            case '(': type = TokenType.LeftParenthesis; break;
            case ')': type = TokenType.RightParenthesis; break;
            case '<': type = TokenType.LeftAngle; break;
            case '>': type = TokenType.RightAngle; break;
            case '^': type = TokenType.Xor; break;
            case '&': type = TokenType.And; break;
            case '|': type = TokenType.Or; break;
            case '~': type = TokenType.Not; break;
            case '.': type = TokenType.Dot; break;
            case ',': type = TokenType.Comma; break;
            case '=': type = TokenType.Assignment; break;
            case '?': type = TokenType.Question; break;
            case ':': type = TokenType.Colon; break;
        }

        if (type != TokenType.Illegal) return new Token(type, reader.GetPosition(), literal.ToString());

        if (literal == '-' && reader.Peek() == '>')
        {
            reader.Read();
            return new Token(TokenType.Arrow, reader.GetPosition(), "->");
        }

        if (literal == '/' && reader.Peek() == '*')
        {
            reader.Read();

            while (true)
            {
                var c = reader.Read();
                if (c == '*' && reader.Peek() == '/')
                {
                    reader.Read();
                    return Consume();
                }

                if (c == '\0') return new Token(TokenType.Illegal, reader.GetPosition(), literal.ToString());
            }
        }

        if (literal == '/' && reader.Peek() == '/')
        {
            reader.Read();
            var initialPos = reader.GetPosition();
            while (true)
            {
                var c1 = reader.Read();
                if (c1 == '\n') return Consume();
                if (c1 == '\0') return new Token(TokenType.End, initialPos, literal.ToString());
            }
        }

        if (IsWhitespace(literal))
        {
            var initialPos = reader.GetPosition();
            while (IsWhitespace(reader.Peek())) reader.Read();
            return Consume();
        }

        if (IsValidIdentifier(literal))
        {
            var initialPos = reader.GetPosition();
            var identifier = literal.ToString();
            while (IsValidIdentifier(reader.Peek())) identifier += reader.Read();

            return identifier.ToLower() switch
            {
                "return" => new Token(TokenType.Return, initialPos, identifier),
                "as" => new Token(TokenType.As, initialPos, identifier),
                _ when IsTypename(identifier) => new Token(TokenType.Type, initialPos, identifier),
                _ => new Token(TokenType.Identifier, initialPos, identifier)
            };
        }

        return new Token(TokenType.Illegal, reader.GetPosition(), literal.ToString());
    }

    private bool IsWhitespace(char c)
    {
        return c == ' ' || c == '\t' || c == '\n' || c == '\r';
    }

    private bool IsTypename(string identifier)
    {
        if (identifier.Length == 1) return char.IsUpper(identifier.First());
        return char.IsUpper(identifier.First()) && identifier.Any(char.IsLower);
    }

    private bool IsValidIdentifier(char c)
    {
        return (c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';
    }

    public Token Peek(int n = 1)
    {
        return peeked.Skip(n - 1).FirstOrDefault() ?? new Token(TokenType.End, reader.GetPosition(), string.Empty);
    }
}