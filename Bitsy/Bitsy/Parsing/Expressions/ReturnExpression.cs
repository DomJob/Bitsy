namespace Bitsy.Parsing.Expressions;

public class ReturnExpression : Expression
{
    public ReturnExpression(Expression expression)
    {
        Expression = expression;
    }

    public Expression Expression { get; }

    public override string ToString()
    {
        return $"return {Expression}";
    }
}