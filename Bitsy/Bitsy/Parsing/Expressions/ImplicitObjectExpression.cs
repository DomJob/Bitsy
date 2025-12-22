using Bitsy.Reading;

namespace Bitsy.Parsing.Expressions;

public class ImplicitObjectExpression : Expression
{
    public ImplicitObjectExpression(List<Expression> body)
    {
        Body = body;
    }

    public List<Expression> Body { get; }

    public override Position Position => Body[0].Position;

    public override string ToString()
    {
        return "{" + string.Join(", ", Body) + "}";
    }
}