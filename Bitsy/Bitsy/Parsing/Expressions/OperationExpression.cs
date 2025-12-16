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
        if(operation.Type == TokenType.Dot)
            return $"({left}.{right})";
        if(operation.Type == TokenType.Assignment)
            return $"{left} = {right}";
        return $"({left} {operation.Literal} {right})";
    }
}