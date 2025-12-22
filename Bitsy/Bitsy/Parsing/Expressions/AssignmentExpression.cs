namespace Bitsy.Parsing.Expressions;

public class AssignmentExpression : Expression
{
    public NameExpression Name { get; set; }
    
    public Expression Expression { get; set; }

    public AssignmentExpression(NameExpression name, Expression expression)
    {
        Name = name;
        Expression = expression;
    }

    public override string ToString() => $"{Name} = {Expression}";
}