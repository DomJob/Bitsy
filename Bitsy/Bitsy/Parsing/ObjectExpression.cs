namespace Bitsy.Parsing;

public class ObjectExpression : Expression
{
    public List<Expression> Expressions { get; }
    
    public ObjectExpression(List<Expression> expressions)
    {
        Expressions = expressions;
    }
    
    public void Add(Expression expression) => Expressions.Add(expression);
    
    public override string ToString() => $"[{string.Join(", ", Expressions)}]";
}