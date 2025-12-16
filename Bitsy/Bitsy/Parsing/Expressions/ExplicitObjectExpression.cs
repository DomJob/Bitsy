namespace Bitsy.Parsing.Expressions;

public class ExplicitObjectExpression : Expression
{
    private List<(Expression, Expression)> body;

    public ExplicitObjectExpression(List<(Expression, Expression)> body)
    {
        this.body = body;
    }

    public override string ToString()
    {
        return "{" + string.Join(", ", body.Select(v => $"{v.Item1}: {v.Item2}")) + "}";
    }
}