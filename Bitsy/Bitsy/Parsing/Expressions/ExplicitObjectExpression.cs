using Bitsy.Reading;

namespace Bitsy.Parsing.Expressions;

public class ExplicitObjectExpression : Expression
{
    public ExplicitObjectExpression(List<(NameExpression, Expression)> body)
    {
        Body = body;
    }

    public List<(NameExpression, Expression)> Body { get; }

    public override Position Position => Body[0].Item1.Position;

    public override string ToString()
    {
        return "{" + string.Join(", ", Body.Select(v => $"{v.Item1}: {v.Item2}")) + "}";
    }
}