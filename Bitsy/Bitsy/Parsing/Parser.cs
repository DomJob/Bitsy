using Bitsy.Lexing;
using Bitsy.Parsing.Expressions;
using Bitsy.Parsing.Statements;

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

    public Statement ParseStatement()
    {
        var identifier = ParseExpression(0, [TokenType.Assignment]);

        var op = Next();

        if (op.Type == TokenType.Assignment)
        {
            var expression = ParseExpression(0, [TokenType.Identifier, TokenType.End]);
            return new BindingStatement(identifier, expression);
        }
        
        return null;
    }
    
    public Expression ParseExpression(int minBindingPower = 0, List<TokenType>? possibleEnds = null)
    {
        if (possibleEnds is null) possibleEnds = [TokenType.End];

        var token = Next();
        var left = ParsePrefix(token, possibleEnds);

        while (true)
        {
            var op = Peek();
            if (possibleEnds.Contains(op.Type)) break;

            if (op.Type == TokenType.LeftParenthesis)
            {
                var (leftBp, _) = InfixBindingPower(op);
                if (leftBp < minBindingPower) break;

                Next(); // consume '('
                var args = ParseExpressions(TokenType.RightParenthesis);
                Next(); // consume ')'
                left = new CallExpression(left, args);
            }
            else
            {
                var (leftBp, rightBp) = InfixBindingPower(op);
                if (leftBp < minBindingPower) break;

                Next(); // Consume op

                if (op.Type == TokenType.As)
                {
                    var identifier = Next();
                    if (identifier.Type != TokenType.Identifier)
                        throw new ParserException("Expected identifier", identifier);

                    left = new AsExpression(left, new IdentifierExpression(identifier));
                }
                else if (op.Type == TokenType.Dot)
                {
                    var identifier = Next();
                    if (identifier.Type != TokenType.Identifier)
                        throw new ParserException("Expected identifier", identifier);

                    left = new DotExpression(left, new IdentifierExpression(identifier));
                }
                else if (op.Type == TokenType.Question)
                {
                    var whenTrue = ParseExpression(rightBp, [TokenType.Colon]);
                    Next(); // consume ':'
                    var whenFalse = ParseExpression(rightBp, possibleEnds);
                    left = new ConditionalExpression(left, whenTrue, whenFalse);
                }
                else
                {
                    left = new InfixExpression(left, op, ParseExpression(rightBp, possibleEnds));
                }
            }

            //throw new ParserException($"Unexpected token. Expected one of {string.Join(", ",possibleEnds)}", token);
        }

        return left;
    }

    private (int leftBp, int rightBp) InfixBindingPower(Token operation)
    {
        switch (operation.Type)
        {
            case TokenType.LeftParenthesis: return (18, 19); // function calls - highest
            case TokenType.Dot: return (16, 17);             // member access
            case TokenType.And: return (10, 11);             // bitwise AND
            case TokenType.Xor: return (8, 9);               // bitwise XOR
            case TokenType.Or: return (6, 7);                // bitwise OR
            case TokenType.Question: return (4, 3);          // ternary (right-assoc)
            case TokenType.As: return (1, 2);                // type cast (left-assoc, LOWEST)
        }

        throw new ParserException("Expected InfixOperator token", operation);
    }

    private Expression ParsePrefix(Token token, List<TokenType> possibleEnds)
    {
        if (IsPrefixOperator(token))
        {
            var bp = PrefixBindingPower(token);
            var operand = ParseExpression(bp, possibleEnds);
            return new PrefixExpression(token, operand);
        }

        if (token.Type == TokenType.LeftParenthesis)
        {
            var expr = ParseExpression(0, [TokenType.RightParenthesis]);
            Next();
            return expr;
        }

        if (token.Type == TokenType.LeftBracket)
        {
            var args = ParseExpressions(TokenType.RightBracket);
            Next();
            return new ObjectExpression(args);
        }

        if (token.Type != TokenType.Identifier)
            throw new ParserException("Expected identifier", token);

        return new IdentifierExpression(token);
    }

    private List<Expression> ParseExpressions(TokenType endSignal)
    {
        var args = new List<Expression>();

        if (Peek().Type == endSignal)
            return args;

        args.Add(ParseExpression(0, [endSignal, TokenType.Comma]));

        while (Peek().Type == TokenType.Comma)
        {
            Next(); // consume ','
            args.Add(ParseExpression(0, [endSignal, TokenType.Comma]));
        }

        return args;
    }


    private int PrefixBindingPower(Token token)
    {
        return 14;
    }

    private bool IsPrefixOperator(Token token)
    {
        return token.Type == TokenType.Not;
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