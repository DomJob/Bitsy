using Bitsy.Lexing;

namespace Bitsy.Parsing.Expressions;

public class OperationExpression : Expression
{
    public Expression Left { get; }
    public Token Operation { get; }
    public Expression Right { get; }

    public OperationExpression(Expression left, Token operation, Expression right)
    {
        Left = left;
        Operation = operation;
        Right = right;
    }

    public override string ToString()
    {
        if(Operation.Type == TokenType.Dot)
            return $"({Left}.{Right})";
        if(Operation.Type == TokenType.Assignment)
            return $"{Left} = {Right}";
        return $"({Left} {Operation.Literal} {Right})";
    }
}