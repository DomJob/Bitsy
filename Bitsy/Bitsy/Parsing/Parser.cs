using Bitsy.Lexing;
using Bitsy.Parsing.Parselets;

namespace Bitsy.Parsing;

public class Parser
{
    private readonly Dictionary<TokenType, InfixParselet> infixParselets = new();
    private readonly Lexer lexer;
    private readonly Dictionary<TokenType, PrefixParselet> prefixParselets = new();

    public Parser(Lexer lexer)
    {
        this.lexer = lexer;

        Register(TokenType.LeftBrace, new ObjectParselet());
        Register(TokenType.Identifier, new NameParselet());
        Register(TokenType.LeftParenthesis, new GroupParselet());
        Register(TokenType.LeftParenthesis, new FunctionParselet());
        Register(TokenType.Question, new ConditionalParselet());
        Register(TokenType.Arrow, new TypeNameParselet());
        Register(TokenType.LeftBrace, new TypeParselet());
        Register(TokenType.LeftAngle, new TypeParselet());
        Prefix(TokenType.Return, BindingPower.Return);
        Prefix(TokenType.Not, BindingPower.Not);
        Infix(TokenType.And, BindingPower.And);
        Infix(TokenType.Or, BindingPower.Or);
        Infix(TokenType.Xor, BindingPower.Xor);
        Infix(TokenType.Dot, BindingPower.Dot);
        Infix(TokenType.Assignment, BindingPower.Assign);
        Infix(TokenType.As, BindingPower.As);
    }

    public Expression ParseExpression()
    {
        return ParseExpression(0);
    }

    public Expression ParseExpression(int precedence)
    {
        var token = Consume();
        var position = token.Position;
        if (!prefixParselets.TryGetValue(token.Type, out var prefix))
            throw new ParserException("Could not parse token", token);

        var left = prefix.Parse(this, token);

        while (precedence < GetPrecedence())
        {
            token = Consume();

            var infix = infixParselets[token.Type];
            left = infix.Parse(this, left, token);
        }
        
        left.Position = position;
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
        var token = Peek();
        if (token.Type != expected) throw new ParserException("Expected " + expected, token);

        return Consume();
    }

    private void Register(TokenType token, PrefixParselet parselet)
    {
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