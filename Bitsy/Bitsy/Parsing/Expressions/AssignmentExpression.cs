using Bitsy.Reading;

namespace Bitsy.Parsing.Expressions;

public class AssignmentExpression : Expression
{
    public AssignmentExpression(NameExpression name, Expression expression)
    {
        Name = name;
        Expression = expression;
    }

    public NameExpression Name { get; set; }

    public Expression Expression { get; set; }

    public override Position Position => Name.Position;

    public override string ToString()
    {
        return $"{Name} = {Expression}";
    }
}