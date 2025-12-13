namespace Bitsy.Parsing;

public class ConditionalExpression : Expression
{
    public Expression Condition { get; set; }
    public Expression WhenTrue { get; set; }
    public Expression WhenFalse { get; set; }

    public ConditionalExpression(Expression condition, Expression whenTrue, Expression whenFalse)
    {
        Condition = condition;
        WhenTrue = whenTrue;
        WhenFalse = whenFalse;
    }
    
    public override string ToString() => $"({Condition} ? {WhenTrue} : {WhenFalse})";
}