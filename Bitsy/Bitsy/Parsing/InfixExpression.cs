using Bitsy.Lexing;

namespace Bitsy.Parsing;

public class InfixExpression : Expression
{
    public Expression Left { get; set; }
    public Token Operator { get; set; }
    public Expression Right { get; set; }

    public InfixExpression(Expression left, Token @operator, Expression right)
    {
        Left = left;
        Operator = @operator;
        Right = right;
    }

    public override string ToString()
    {
        return $"({Left} {Operator.Literal} {Right})";
    }
}