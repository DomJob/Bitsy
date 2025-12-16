namespace Bitsy.Parsing.Expressions;

public class ExplicitObjectExpression : Expression
{
    public List<(Expression, Expression)> Body { get; }

    public ExplicitObjectExpression(List<(Expression, Expression)> body)
    {
        Body = body;
    }

    public override string ToString()
    {
        return "{" + string.Join(", ", Body.Select(v => $"{v.Item1}: {v.Item2}")) + "}";
    }
}