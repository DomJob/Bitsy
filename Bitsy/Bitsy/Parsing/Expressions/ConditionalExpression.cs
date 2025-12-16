namespace Bitsy.Parsing.Expressions;

public class ConditionalExpression : Expression
{
    public Expression Condition { get; }
    public Expression IfTrue { get; }
    public Expression IfFalse { get; }

    public ConditionalExpression(Expression condition, Expression ifTrue, Expression ifFalse)
    {
        Condition = condition;
        IfTrue = ifTrue;
        IfFalse = ifFalse;
    }
    
    public override string ToString() => $"({Condition} ? {IfTrue} : {IfFalse})";
}