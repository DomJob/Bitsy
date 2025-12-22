using Bitsy.Reading;

namespace Bitsy.Parsing.Expressions;

public class CallExpression : Expression
{
    public CallExpression(Expression expression, List<Expression> arguments)
    {
        Expression = expression;
        Arguments = arguments;
    }

    public Expression Expression { get; }

    public List<Expression> Arguments { get; }

    public override Position Position => Expression.Position;

    public override string ToString()
    {
        return $"{Expression}({string.Join(", ", Arguments)})";
    }
}