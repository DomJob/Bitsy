using Bitsy.Lexing;

namespace Bitsy.Parsing.Expressions;

public class OperationExpression : Expression
{
    private readonly Expression left;
    private readonly Token operation;
    private readonly Expression right;

    public OperationExpression(Expression left, Token operation, Expression right)
    {
        this.left = left;
        this.operation = operation;
        this.right = right;
    }

    public override string ToString()
    {
        return $"({left} {operation.Literal} {right})";
    }
}