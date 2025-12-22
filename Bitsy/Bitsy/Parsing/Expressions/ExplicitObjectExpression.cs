using Bitsy.Reading;

namespace Bitsy.Parsing.Expressions;

public class ExplicitObjectExpression : Expression
{
    public ExplicitObjectExpression(List<(Expression, Expression)> body)
    {
        Body = body;

        foreach (var (name, _) in body) // TODO
            if (name.GetType() != typeof(NameExpression))
                throw new SyntaxError("Property in explicit object must be an identifier", name.Position);
    }

    public List<(Expression, Expression)> Body { get; }

    public override Position Position => Body[0].Item1.Position;

    public override string ToString()
    {
        return "{" + string.Join(", ", Body.Select(v => $"{v.Item1}: {v.Item2}")) + "}";
    }
}