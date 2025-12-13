using Bitsy.Lexing;

namespace Bitsy.Parsing.Expressions;

public class InfixExpression : Expression
{
    public InfixExpression(Expression left, Token operation, Expression right)
    {
        Left = left;
        Operation = operation;
        Right = right;
    }

    public Expression Left { get; set; }
    public Token Operation { get; set; }
    public Expression Right { get; set; }

    public override string ToString()
    {
        return $"({Left} {Operation.Literal} {Right})";
    }
}