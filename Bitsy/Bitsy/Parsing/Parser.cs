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

        Register(TokenType.LeftBrace, new ObjectParselet());
        Register(TokenType.Identifier, new NameParselet());
        Register(TokenType.LeftParenthesis, new GroupParselet());
        Register(TokenType.LeftParenthesis, new FunctionCallParselet());
        Register(TokenType.Question, new ConditionalParselet());
        Prefix(TokenType.Return, BindingPower.Return);
        Prefix(TokenType.Not, BindingPower.Not);
        Infix(TokenType.And, BindingPower.And);
        Infix(TokenType.Or, BindingPower.Or);
        Infix(TokenType.Xor, BindingPower.Xor);
        Infix(TokenType.Dot, BindingPower.Dot);
        Infix(TokenType.As, BindingPower.As);
    }

    public Expression ParseStatement()
    {
        var firstToken = Consume();

        if (firstToken.Type == TokenType.Return)
            return new UnaryExpression(firstToken, ParseExpression());

        var nameExpr = new NameExpression(firstToken);

        var nextToken = Peek();

        switch (nextToken.Type)
        {
            case TokenType.Assignment:
                return new BinaryExpression(nameExpr, Consume(), ParseExpression());
            case TokenType.LeftParenthesis: // Function declaration
                Consume();
                return ParseFunctionDeclaration(nameExpr);
            case TokenType.LeftBrace: // Non-generic type definition
            case TokenType.LeftAngle: // Generic type definition
                return ParseTypeDefinition(nameExpr);
        }

        throw new ParserException("Unexpected token in start of statement", firstToken);
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

        if (unionPossible && Peek().Type == TokenType.Comma)
        {
            var types = new List<TypeExpression> { left! };
            while (Match(TokenType.Comma))
                types.Add(ParseType());
            left = new UnionTypeExpression(types);
        }

        if (Match(TokenType.Arrow)) left = new FunctionTypeExpression(left!, ParseType());

        return left;
    }

    private TypeDeclaration ParseTypeDefinition(NameExpression name)
    {
        List<TypeExpression> templates = [];

        if (Peek().Type == TokenType.LeftAngle)
        {
            Consume();
            while (true)
            {
                templates.Add(ParseType(false));
                if (Match(TokenType.RightAngle)) break;
                Consume(TokenType.Comma);
            }
        }

        Consume(TokenType.LeftBrace);
        List<(TypeExpression Type, NameExpression Name)> definition = [];

        while (true)
        {
            if (Match(TokenType.RightBrace)) break;
            definition.Add((ParseType(), ParseName()));
        }

        return new TypeDeclaration(name, templates, definition);
    }

    private FunctionDeclaration ParseFunctionDeclaration(NameExpression name)
    {
        List<(TypeExpression, NameExpression)> arguments = [];

        while (true)
        {
            if (Match(TokenType.RightParenthesis)) break;
            arguments.Add((ParseType(), ParseName()));
            Consume(TokenType.Comma);
        }

        Consume(TokenType.LeftBrace);

        List<Expression> body = [];
        while (!Match(TokenType.RightBrace))
            body.Add(ParseStatement());

        return new FunctionDeclaration(name, arguments, body);
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