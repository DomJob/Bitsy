namespace Bitsy.Parsing.Expressions;

public class CallExpression : Expression
{
    public CallExpression(Expression target, List<Expression> arguments)
    {
        Target = target;
        Arguments = arguments;
    }

    public Expression Target { get; }
    public List<Expression> Arguments { get; }

    public override string ToString()
    {
        return $"{Target}({string.Join(", ", Arguments)})";
    }
}