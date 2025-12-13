namespace Bitsy.Parsing.Expressions;

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
    
    public override string Details(int indent = 0)
    {
        var tab = new string(' ', indent);

        return $"{tab}[Conditional\n" +
               $"{Condition.Details(indent + 1)}\n" +
               $"{WhenTrue.Details(indent + 1)}\n" +
               $"{WhenFalse.Details(indent + 1)}\n" +
               $"{tab}]";
    }
}