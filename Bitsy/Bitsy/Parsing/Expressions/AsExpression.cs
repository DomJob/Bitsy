namespace Bitsy.Parsing.Expressions;

public class AsExpression : Expression
{
    public Expression Expression { get; }
    
    public IdentifierExpression Identifier { get; }

    public AsExpression(Expression expression, IdentifierExpression identifier)
    {
        Expression = expression;
        Identifier = identifier;
    }

    public override string ToString() => $"({Expression} as {Identifier})";
}