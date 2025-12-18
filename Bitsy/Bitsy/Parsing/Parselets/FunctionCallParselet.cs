using Bitsy.Lexing;
using Bitsy.Parsing.Expressions;

namespace Bitsy.Parsing.Parselets;

public class FunctionCallParselet : InfixParselet
{
    public Expression Parse(Parser parser, Expression left, Token token)
    {
        var args = new List<Expression>();

        if(parser.Match(TokenType.RightParenthesis))
            return new CallExpression(left, args);
        
        do
        {
            args.Add(parser.ParseExpression());
        } while (parser.Match(TokenType.Comma));

        parser.Consume(TokenType.RightParenthesis);

        return new CallExpression(left, args);
    }

    public int Precedence => BindingPower.Call;
}