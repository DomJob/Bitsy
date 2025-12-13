namespace Bitsy.Parsing.Expressions;

public class DotExpression : Expression
{
    public DotExpression(Expression o, IdentifierExpression attribute)
    {
        Object = o;
        Attribute = attribute;
    }

    public Expression Object { get; }
    public IdentifierExpression Attribute { get; }

    public override string ToString()
    {
        return $"({Object}.{Attribute})";
    }
}