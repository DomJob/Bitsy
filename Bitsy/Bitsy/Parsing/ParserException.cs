using Bitsy.Lexing;

namespace Bitsy.Parsing;

public class ParserException : Exception
{
    public ParserException(string message, Token token) : base(message)
    {
        Token = token;
    }

    public ParserException(string message) : base(message)
    {
    }

    public Token? Token { get; }
}