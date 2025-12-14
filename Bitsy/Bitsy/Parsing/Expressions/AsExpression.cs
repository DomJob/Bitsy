namespace Bitsy.Parsing.Expressions;

public class AsExpression : Expression
{
    public AsExpression(Expression expression, IdentifierExpression identifier)
    {
        Expression = expression;
        Identifier = identifier;
    }

    public Expression Expression { get; }

    public IdentifierExpression Identifier { get; }

    public override string ToString()
    {
        return $"({Expression} as {Identifier})";
    }
}