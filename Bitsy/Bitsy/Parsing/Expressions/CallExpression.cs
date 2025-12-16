namespace Bitsy.Parsing.Expressions;

public class CallExpression : Expression
{
    public Expression Expression { get; }
    
    public List<Expression> Arguments { get; }

    public CallExpression(Expression expression, List<Expression> arguments)
    {
        Expression = expression;
        Arguments = arguments;
    }
    
    public override string ToString() => $"{Expression}({string.Join(", ", Arguments)})";
}