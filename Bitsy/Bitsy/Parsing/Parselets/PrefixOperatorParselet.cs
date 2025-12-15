using Bitsy.Lexing;
using Bitsy.Parsing.Expressions;

namespace Bitsy.Parsing.Parselets;

public class PrefixOperatorParselet : IPrefixParselet
{
    public Expression Parse(Parser parser, Token token)
    {
        Expression operand = parser.ParseExpression();
        return new PrefixExpression(token, operand);
    }
}