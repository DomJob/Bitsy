using Bitsy.Lexing;
using Bitsy.Parsing.Expressions;

namespace Bitsy.Parsing.Parselets;

public class ConditionalParselet : InfixParselet
{
    public Expression Parse(Parser parser, Expression left, Token token)
    {
        var ifTrue = parser.ParseExpression();
        parser.Consume(TokenType.Colon);
        var ifFalse = parser.ParseExpression();
        
        return new ConditionalExpression(left, ifTrue, ifFalse);
    }

    public int Precedence => BindingPower.Conditional;
}