namespace Bitsy.Parsing;

public class DotExpression : Expression
{
    public Expression Object { get; }
    public IdentifierExpression Attribute { get; }

    public DotExpression(Expression o, IdentifierExpression attribute)
    {
        Object = o;
        Attribute = attribute;
    }
    
    public override string ToString() => $"{Object}.{Attribute}";
}