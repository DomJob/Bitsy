using Bitsy.Reading;

namespace Bitsy.Lexing;

public class Lexer
{
    private readonly IReader reader;
    
    public Lexer(IReader reader)
    {
        this.reader = reader;
    }

    public Token Next()
    {
        if (!reader.HasMore()) return new Token(TokenType.End, reader.GetPosition());

        var literal = reader.Read();
        
        TokenType type = TokenType.Illegal;
        
        switch (literal)
        {
            case '{': type = TokenType.LeftBrace; break;
            case '}': type = TokenType.RightBrace; break;
            case '[': type = TokenType.LeftBracket; break;
            case ']': type = TokenType.RightBracket; break;
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
        if(type != TokenType.Illegal) return new Token(type, reader.GetPosition(), literal.ToString());

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
                    return Next();
                }
                
                if (c=='\0') return new Token(TokenType.Illegal, reader.GetPosition(), literal.ToString());
            }
        }

        if (literal == '/' && reader.Peek() == '/')
        {
            reader.Read();
            var initialPos = reader.GetPosition();
            while (true)
            {
                var c1 = reader.Read();
                if (c1 == '\n') return Next();
                if (c1 == '\0') return new Token(TokenType.End, initialPos, literal.ToString());
            }
        }

        if (IsWhitespace(literal))
        {
            var initialPos = reader.GetPosition();
            while(IsWhitespace(reader.Peek())) reader.Read();
            return Next();
        }

        if (IsValidIdentifier(literal))
        {
            var initialPos = reader.GetPosition();
            String identifier = literal.ToString();
            while (IsValidIdentifier(reader.Peek()))
            {
                identifier += reader.Read();
            }

            return identifier.ToLower() switch
            {
                "return" => new Token(TokenType.Return, initialPos, identifier),
                "as" => new Token(TokenType.As, initialPos, identifier),
                _ => new Token(TokenType.Identifier, initialPos, identifier)
            };
        }
        
        return new Token(TokenType.Illegal, reader.GetPosition(), literal.ToString());
    }

    private bool IsWhitespace(char c)
    {
        return c == ' ' || c == '\t' || c == '\n' || c == '\r';
    }

    private bool IsValidIdentifier(char c)
    {
        return (c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';
    }
}