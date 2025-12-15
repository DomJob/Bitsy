using Bitsy.Lexing;

namespace Bitsy.Parsing;

public class Parser
{
    private readonly Lexer lexer;

    public Parser(Lexer lexer)
    {
        this.lexer = lexer;
    }

    private Token Peek()
    {
        return lexer.Peek();
    }

    private Token Next()
    {
        return lexer.Next();
    }

    public Expression Parse()
    {
        return null;
    }
}

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