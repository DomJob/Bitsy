using Bitsy.Lexing;
using Bitsy.Parsing.Expressions;

namespace Bitsy.Parsing.Parselets;

public class PrefixOperatorParselet : PrefixParselet
{
    public PrefixOperatorParselet(int precedence)
    {
        Precedence = precedence;
    }

    public int Precedence { get; }

    public Expression Parse(Parser parser, Token token)
    {
        var operand = parser.ParseExpression(Precedence);
        return new UnaryExpression(token, operand);
    }
}