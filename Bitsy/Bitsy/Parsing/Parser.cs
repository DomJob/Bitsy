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

    public Expression ParseStatement()
    {
        if (NextTokenIs(TokenType.Return))
            return ParseReturnStatement();
        if (!NextTokenIs(TokenType.Identifier))
            return ParseExpression();

        switch (Peek(2).Type)
        {
            case TokenType.Assignment:
                return ParseAssignment();
            case TokenType.LeftParenthesis:
                return ParseFunction();
            case TokenType.LeftBrace:
            case TokenType.LeftAngle:
                return ParseTypeDefinition();
            case TokenType.Arrow:
                return ParseType();
            default:
                return ParseExpression();
        }
    }

    private BinaryExpression ParseAssignment()
    {
        return new BinaryExpression(ParseName(), Consume(TokenType.Assignment), ParseExpression());
    }

    private UnaryExpression ParseReturnStatement()
    {
        return new UnaryExpression(Consume(TokenType.Return), ParseExpression());
    }

    public TypeExpression ParseType(bool unionPossible = true)
    {
        var token = Consume();

        TypeExpression left = null;

        switch (token.Type)
        {
            case TokenType.Identifier:
                List<SimpleTypeExpression> templates = [];

                if (Match(TokenType.LeftAngle))
                    while (true)
                    {
                        var templateToken = Consume(TokenType.Identifier);
                        templates.Add(new SimpleTypeExpression(templateToken));
                        if (Match(TokenType.RightAngle)) break;
                        Consume(TokenType.Comma);
                    }

                left = new SimpleTypeExpression(token, templates);

                break;
            case TokenType.LeftParenthesis when Match(TokenType.RightParenthesis):
                left = new UnionTypeExpression([]);
                break;
            case TokenType.LeftParenthesis:
                left = ParseType();
                Consume(TokenType.RightParenthesis);
                break;
            case TokenType.RightParenthesis:
                break;
            default:
                throw new ParserException("Unexpected token when parsing type", token);
        }

        if (unionPossible && NextTokenIs(TokenType.Comma))
        {
            var types = new List<TypeExpression> { left! };
            while (Match(TokenType.Comma))
                types.Add(ParseType());
            left = new UnionTypeExpression(types);
        }

        if (Match(TokenType.Arrow)) left = new FunctionTypeExpression(left!, ParseType());

        return left;
    }

    private Expression ParseTypeDefinition()
    {
        var name = ParseName();
        List<TypeExpression> templates = [];

        if (NextTokenIs(TokenType.LeftAngle))
        {
            Consume();
            while (true)
            {
                templates.Add(ParseType(false));
                if (Match(TokenType.RightAngle)) break;
                Consume(TokenType.Comma);
            }
        }

        if (!Match(TokenType.LeftBrace))
        {
            var actualType =
                new SimpleTypeExpression(name.Name, templates.Select(t => (SimpleTypeExpression)t).ToList());
            if (Match(TokenType.Arrow))
                return new FunctionTypeExpression(actualType, ParseType());
            return actualType;
        }

        List<(TypeExpression Type, NameExpression Name)> definition = [];

        while (true)
        {
            if (Match(TokenType.RightBrace)) break;
            definition.Add((ParseType(), ParseName()));
        }

        return new TypeDeclaration(name, templates, definition);
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

        if (firstExpression is NameExpression paramTypeName)
            arguments.Add((paramTypeName.ToType(), parameterName));
        else if (firstExpression is TypeExpression paramType)
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
                arguments.Add((ParseType(), ParseName()));
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
            body.Add(ParseStatement());
        Match(TokenType.RightBrace);
        return body;
    }

    private NameExpression ParseName()
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
            left.Position = token.Position;
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

    public bool Match(TokenType expected)
    {
        if (!NextTokenIs(expected)) return false;
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