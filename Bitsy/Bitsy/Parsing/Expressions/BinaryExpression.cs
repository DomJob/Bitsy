using Bitsy.Lexing;

namespace Bitsy.Parsing.Expressions;

public class BinaryExpression : Expression
{
    public BinaryExpression(Expression left, Token operation, Expression right)
    {
        Left = left;
        Operation = operation;
        Right = right;
    }

    public Expression Left { get; }
    public Token Operation { get; }
    public Expression Right { get; }

    public override string ToString()
    {
        if (Operation.Type == TokenType.Dot)
            return $"({Left}.{Right})";
        if (Operation.Type == TokenType.Assignment)
            return $"{Left} = {Right}";
        return $"({Left} {Operation.Literal} {Right})";
    }
}