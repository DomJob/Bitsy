namespace Bitsy.Parsing.Expressions;

public class ObjectExpression : Expression
{
    public ObjectExpression(List<Expression> expressions)
    {
        Expressions = expressions;
    }

    public List<Expression> Expressions { get; }

    public void Add(Expression expression)
    {
        Expressions.Add(expression);
    }

    public override string ToString()
    {
        return $"[{string.Join(", ", Expressions)}]";
    }
}