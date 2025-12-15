using Bitsy.Lexing;
using Bitsy.Parsing.Parselets;

namespace Bitsy.Parsing;

public class Parser
{
    private readonly Lexer lexer;
    private readonly Dictionary<TokenType, IPrefixParselet> prefixParselets = new();
    
    public Parser(Lexer lexer)
    {
        this.lexer = lexer;
        
        Register(TokenType.Identifier, new NameParselet());
        Prefix(TokenType.Not);
    }
    
    public Expression ParseExpression()
    {
        var token = Consume();
        var prefix = prefixParselets[token.Type];
        if (prefix == null)
            throw new ParserException("Could not parse token", token);

        return prefix.Parse(this, token);
    }
    
    private Token Peek()
    {
        return lexer.Peek();
    }

    private Token Consume()
    {
        return lexer.Next();
    }

    private void Register(TokenType token, IPrefixParselet parselet) {
        prefixParselets[token] = parselet;
    }

    private void Prefix(TokenType token) {
        Register(token, new PrefixOperatorParselet());
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