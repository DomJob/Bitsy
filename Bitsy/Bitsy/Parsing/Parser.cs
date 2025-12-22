using Bitsy.Lexing;
using Bitsy.Parsing.Expressions;
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

        RegisterPrefixTokens();
        RegisterInfixTokens();
    }

    public Expression Next()
    {
        if (NextTokenIs(TokenType.Return))
            return ParseReturn();

        return Peek().Type switch
        {
            TokenType.Identifier when Peek(2).Type == TokenType.Assignment => ParseAssignment(),
            TokenType.Identifier when Peek(2).Type == TokenType.LeftParenthesis => ParseFunction(),
            _ => ParseExpression()
        };
    }

    private AssignmentExpression ParseAssignment()
    {
        var name = ParseName();
        Consume(TokenType.Assignment);
        var expression = ParseExpression();
        return new AssignmentExpression(name, expression);
    }

    private ReturnExpression ParseReturn()
    {
        var returnToken = Consume(TokenType.Return);
        return new ReturnExpression(returnToken, ParseExpression());
    }

    public TypeExpression ParseTypeSignature()
    {
        var token = Consume();

        TypeExpression left;

        switch (token.Type)
        {
            case TokenType.Type:
                left = new SimpleTypeExpression(token, ParseTemplates());
                break;
            case TokenType.LeftParenthesis when Match(TokenType.RightParenthesis):
                left = new UnionTypeExpression(token.Position);
                break;
            case TokenType.LeftParenthesis:
                left = ParseTypeSignature();
                Consume(TokenType.RightParenthesis);
                break;
            default:
                throw new ParserException("Unexpected token when parsing type", token);
        }

        if (NextTokenIs(TokenType.Comma))
        {
            var types = new List<TypeExpression> { left! };
            while (Match(TokenType.Comma))
                types.Add(ParseTypeSignature());
            left = new UnionTypeExpression(types);
        }

        if (Match(TokenType.Arrow)) left = new FunctionTypeExpression(left!, ParseTypeSignature());

        return left;
    }

    public List<TypeExpression> ParseTemplates(bool ignoreCheck = false)
    {
        if (!ignoreCheck && !Match(TokenType.LeftAngle)) return [];

        List<TypeExpression> templates = [];
        do
        {
            templates.Add(ParseTypeSignature());
            if (Match(TokenType.RightAngle)) break;
        } while (Match(TokenType.Comma));

        return templates;
    }

    private Expression ParseFunction()
    {
        var name = ParseName();
        Consume(TokenType.LeftParenthesis);

        if (Match(TokenType.RightParenthesis))
            return NextTokenIs(TokenType.LeftBrace)
                ? ParseFunctionDefinition(name, [])
                : ParseCallExpression(name, []);

        var firstExpression = ParseExpression();

        if (NextTokenIs(TokenType.Comma, TokenType.RightParenthesis))
            return ParseCallExpression(name, [firstExpression]);

        var parameterName = ParseName();
        List<(TypeExpression, NameExpression)> arguments = [];

        if (firstExpression is TypeExpression paramType)
            arguments.Add((paramType, parameterName));
        else throw new SyntaxError("Expected type", firstExpression.Position);

        return ParseFunctionDefinition(name, arguments);
    }

    private Expression ParseFunctionDefinition(NameExpression name, List<(TypeExpression, NameExpression)> arguments)
    {
        if (Match(TokenType.RightParenthesis))
            return new FunctionDeclaration(name, arguments, ParseFunctionBody());

        if (NextTokenIs(TokenType.Comma))
            do
            {
                Consume(TokenType.Comma);
                arguments.Add((ParseTypeSignature(), ParseName()));
            } while (!Match(TokenType.RightParenthesis));

        return new FunctionDeclaration(name, arguments, ParseFunctionBody());
    }

    private Expression ParseCallExpression(NameExpression name, List<Expression> arguments)
    {
        if (Match(TokenType.RightParenthesis))
            return new CallExpression(name, arguments);
        while (Match(TokenType.Comma))
            arguments.Add(ParseExpression());
        return new CallExpression(name, arguments);
    }

    private List<Expression> ParseFunctionBody()
    {
        Match(TokenType.LeftBrace);
        List<Expression> body = [];
        while (!Match(TokenType.RightBrace))
            body.Add(Next());
        Match(TokenType.RightBrace);
        return body;
    }

    public NameExpression ParseName()
    {
        return new NameExpression(Consume(TokenType.Identifier));
    }

    public Expression ParseExpression(int precedence = 0)
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

        return left;
    }

    private int GetPrecedence()
    {
        var token = Peek();
        if (infixParselets.TryGetValue(token.Type, out var infix))
            return infix.Precedence;
        return 0;
    }

    private Token Peek(int n = 1)
    {
        return lexer.Peek(n);
    }

    private Token Consume()
    {
        return lexer.Next();
    }

    private bool NextTokenIs(params TokenType[] tokenType)
    {
        return tokenType.Any(t => t == Peek().Type);
    }

    public bool Match(params TokenType[] tokenType)
    {
        if (!NextTokenIs(tokenType)) return false;
        Consume();
        return true;
    }

    public Token Consume(TokenType expected)
    {
        var token = Peek();
        if (token.Type != expected) throw new ParserException("Expected " + expected, token);

        return Consume();
    }

    private void RegisterInfixTokens()
    {
        Register(TokenType.LeftParenthesis, new FunctionCallParselet());
        Register(TokenType.Question, new ConditionalParselet());
        Infix(TokenType.LeftAngle, BindingPower.Template);
        Infix(TokenType.Arrow, BindingPower.Arrow);
        Infix(TokenType.And, BindingPower.And);
        Infix(TokenType.Or, BindingPower.Or);
        Infix(TokenType.Xor, BindingPower.Xor);
        Infix(TokenType.Dot, BindingPower.Dot);
        Infix(TokenType.As, BindingPower.As);
    }

    private void RegisterPrefixTokens()
    {
        Register(TokenType.LeftParenthesis, new GroupParselet());
        Register(TokenType.LeftBrace, new ObjectParselet());
        Register(TokenType.Identifier, new NameParselet());
        Register(TokenType.Type, new TypeParselet());
        Prefix(TokenType.Return, BindingPower.Return);
        Prefix(TokenType.Not, BindingPower.Not);
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

    private void Infix(TokenType token, int precedence)
    {
        Register(token, new BinaryOperatorParselet(precedence));
    }
}