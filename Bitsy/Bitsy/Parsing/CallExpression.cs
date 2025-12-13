namespace Bitsy.Parsing;

public class CallExpression : Expression
{
    public Expression Target { get; }
    public List<Expression> Arguments { get; }

    public CallExpression(Expression target, List<Expression> arguments)
    {
        Target = target;
        Arguments = arguments;
    }
    
    public override string ToString() => $"{Target}({string.Join(", ", Arguments)})";
}