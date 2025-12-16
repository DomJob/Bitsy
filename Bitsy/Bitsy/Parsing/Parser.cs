using Bitsy.Lexing;
using Bitsy.Parsing.Parselets;

namespace Bitsy.Parsing;

public class Parser
{
    private readonly Lexer lexer;
    private readonly Dictionary<TokenType, PrefixParselet> prefixParselets = new();
    private readonly Dictionary<TokenType, InfixParselet> infixParselets = new();
    
    public Parser(Lexer lexer)
    {
        this.lexer = lexer;
        
        Register(TokenType.Identifier, new NameParselet());
        Register(TokenType.LeftParenthesis, new GroupParselet());
        Register(TokenType.LeftParenthesis, new CallParselet());
        Prefix(TokenType.Not, BindingPower.Not);
        Infix(TokenType.And, BindingPower.And);
        Infix(TokenType.Or, BindingPower.Or);
        Infix(TokenType.Xor, BindingPower.Xor);
    }

    public Expression ParseExpression() => ParseExpression(0);
    
    public Expression ParseExpression(int precedence)
    {
        var token = Consume();
        if (!prefixParselets.TryGetValue(token.Type, out var prefix))
            throw new ParserException("Could not parse token", token);

        var left = prefix.Parse(this, token);

        while (precedence < GetPrecedence()) {
            token = Consume();

            InfixParselet infix = infixParselets[token.Type];
            left = infix.Parse(this, left, token);
        }

        return left;
    }
    
    private int GetPrecedence()
    {
        var token = Peek();
        if (infixParselets.TryGetValue(token.Type, out var infix))
            return infix.Precedence;
        return 0;
    }
    
    private Token Peek()
    {
        return lexer.Peek();
    }

    private Token Consume()
    {
        return lexer.Next();
    }

    public bool Match(TokenType expected)
    {
        var type = Peek().Type;
        if (type != expected) return false;
        Consume();
        return true;
    } 
    
    public Token Consume(TokenType expected)
    {
        Token token = Peek();
        if (token.Type != expected) {
            throw new ParserException("Expected " + expected, token);
        }
    
        return Consume();
    }

    private void Register(TokenType token, PrefixParselet parselet) {
        prefixParselets[token] = parselet;
    }

    private void Register(TokenType token, InfixParselet parselet)
    {
        infixParselets[token] = parselet;
    }

    private void Prefix(TokenType token, int precedence)
    {
        Register(token, new PrefixOperatorParselet(precedence));
    }
    private void Infix(TokenType token, int precedence, bool isRight = false)
    {
        Register(token, new BinaryOperatorParselet(precedence, isRight));
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