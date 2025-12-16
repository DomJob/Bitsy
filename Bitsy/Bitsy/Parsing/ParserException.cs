using Bitsy.Lexing;

namespace Bitsy.Parsing;

public class ParserException : Exception
{
    public ParserException(string message, Token token)
    {
        Message = message;
        Token = token;
    }

    public ParserException(string message)
    {
        Message = message;
    }

    public string Message { get; }
    public Token? Token { get; }
}