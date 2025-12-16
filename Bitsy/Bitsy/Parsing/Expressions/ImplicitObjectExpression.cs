namespace Bitsy.Parsing.Expressions;

public class ImplicitObjectExpression : Expression
{
    public List<Expression> Body { get; }

    public ImplicitObjectExpression(List<Expression> body)
    {
        Body = body;
    }
    
    public override string ToString() => "{" + string.Join(", ", Body) + "}";
}