using Bitsy.Lexing;
using Bitsy.Parsing.Expressions;

namespace Bitsy.Parsing.Parselets;

public class FunctionParselet : InfixParselet
{
    public Expression Parse(Parser parser, Expression left, Token token)
    {
        List<Expression> args = new();

        if (!parser.Match(TokenType.RightParenthesis))
        {
            var expression = parser.ParseExpression();
            if (parser.Match(TokenType.Comma))
                return ParseCall(parser, left, expression);
            if (parser.Match(TokenType.RightParenthesis))
                return new CallExpression(left, [expression]);

            return ParseFunctionDeclaration(parser, left, expression);
        }

        if (parser.Match(TokenType.LeftBrace))
            return ParseFunctionDeclaration(parser, left);

        return new CallExpression(left, args);
    }

    public int Precedence => BindingPower.Call;

    private FunctionDeclaration ParseFunctionDeclaration(Parser parser, Expression name, Expression? argType = null)
    {
        List<(Expression, Expression)> args = [];

        if (argType != null)
        {
            var argName = parser.ParseExpression();
            args.Add((argType, argName));


            while (parser.Match(TokenType.Comma))
            {
                argType = parser.ParseExpression();
                argName = parser.ParseExpression();
                args.Add((argType, argName));
                if (parser.Match(TokenType.RightParenthesis))
                    break;
            }

            parser.Match(TokenType.RightParenthesis);

            parser.Consume(TokenType.LeftBrace);
        }

        List<Expression> body = [];

        while (!parser.Match(TokenType.RightBrace)) body.Add(parser.ParseExpression());

        return new FunctionDeclaration(name, args, body);
    }

    private CallExpression ParseCall(Parser parser, Expression callee, Expression? firstArg)
    {
        if (firstArg == null)
            return new CallExpression(callee, []);

        var args = new List<Expression> { firstArg! };

        do
        {
            args.Add(parser.ParseExpression());
        } while (parser.Match(TokenType.Comma));

        parser.Consume(TokenType.RightParenthesis);

        return new CallExpression(callee, args);
    }
}