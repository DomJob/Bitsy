using Bitsy.Lexing;
using Bitsy.Parsing.Expressions;

namespace Bitsy.Parsing.Parselets;

public class CallParselet : InfixParselet
{
    public Expression Parse(Parser parser, Expression left, Token token)
    {
        List<Expression> args = new();

        // There may be no arguments at all.
        if (!parser.Match(TokenType.RightParenthesis))
        {
            do
            {
                args.Add(parser.ParseExpression());
            } while (parser.Match(TokenType.Comma));

            parser.Consume(TokenType.RightParenthesis);
        }

        return new CallExpression(left, args);
    }

    public int Precedence => BindingPower.Call;
}