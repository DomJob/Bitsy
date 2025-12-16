namespace Bitsy.Parsing.Expressions;

public class ConditionalExpression : Expression
{
    public ConditionalExpression(Expression condition, Expression ifTrue, Expression ifFalse)
    {
        Condition = condition;
        IfTrue = ifTrue;
        IfFalse = ifFalse;
    }

    public Expression Condition { get; }
    public Expression IfTrue { get; }
    public Expression IfFalse { get; }

    public override string ToString()
    {
        return $"({Condition} ? {IfTrue} : {IfFalse})";
    }
}