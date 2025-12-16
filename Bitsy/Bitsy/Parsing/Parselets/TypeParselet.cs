using Bitsy.Lexing;
using Bitsy.Parsing.Expressions;

namespace Bitsy.Parsing.Parselets;

public class TypeParselet : InfixParselet
{
    public int Precedence => BindingPower.Arrow;
    public Expression Parse(Parser parser, Expression left, Token token)
    {
        return new TypeName([left], parser.ParseExpression());
    }
}