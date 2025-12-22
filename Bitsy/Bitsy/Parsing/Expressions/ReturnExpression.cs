namespace Bitsy.Parsing.Expressions;

public class ReturnExpression : Expression
{
    public Expression Expression { get; }

    public ReturnExpression(Expression expression)
    {
        Expression = expression;
    }

    public override string ToString() => $"return {Expression}";
}