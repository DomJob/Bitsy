namespace Bitsy.Parsing.Expressions;

public class DotExpression : Expression
{
    public Expression Object { get; }
    public IdentifierExpression Attribute { get; }

    public DotExpression(Expression o, IdentifierExpression attribute)
    {
        Object = o;
        Attribute = attribute;
    }
    
    public override string ToString() => $"({Object}.{Attribute})";
    
    public override string Details(int indent = 0)
    {
        var tab = new string(' ', indent);
        return $"{tab}[Dot\n{tab+' '}{Object.Details(indent + 1)}\n{tab+' '}{Attribute.Details(indent+1)}]";
    }
}