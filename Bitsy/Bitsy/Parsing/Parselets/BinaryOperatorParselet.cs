using Bitsy.Lexing;
using Bitsy.Parsing.Expressions;

namespace Bitsy.Parsing.Parselets;

public class BinaryOperatorParselet : InfixParselet
{
    public int Precedence { get; }
    private readonly bool isRight;
    
    public BinaryOperatorParselet(int precedence, bool isRight = false)
    {
        Precedence = precedence;
        this.isRight = isRight;
    }
    
    public Expression Parse(Parser parser, Expression left, Token token)
    {
        var right = parser.ParseExpression(Precedence - (isRight ? 1 : 0));
        return new OperationExpression(left, token, right);
    }
}