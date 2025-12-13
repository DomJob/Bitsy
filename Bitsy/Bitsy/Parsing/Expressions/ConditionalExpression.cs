namespace Bitsy.Parsing.Expressions;

public class ConditionalExpression : Expression
{
    public ConditionalExpression(Expression condition, Expression whenTrue, Expression whenFalse)
    {
        Condition = condition;
        WhenTrue = whenTrue;
        WhenFalse = whenFalse;
    }

    public Expression Condition { get; set; }
    public Expression WhenTrue { get; set; }
    public Expression WhenFalse { get; set; }

    public override string ToString()
    {
        return $"({Condition} ? {WhenTrue} : {WhenFalse})";
    }
}