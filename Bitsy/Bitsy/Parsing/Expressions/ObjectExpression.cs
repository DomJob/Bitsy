namespace Bitsy.Parsing.Expressions;

public class ObjectExpression : Expression
{
    public List<Expression> Expressions { get; }
    
    public ObjectExpression(List<Expression> expressions)
    {
        Expressions = expressions;
    }
    
    public void Add(Expression expression) => Expressions.Add(expression);
    
    public override string ToString() => $"[{string.Join(", ", Expressions)}]";
    
    public override string Details(int indent = 0)
    {
        var tab = new string(' ', indent);
        var str = $"{tab}[Object\n";
        for(var i = 0; i < Expressions.Count; i++)
        {
            str += $"{tab}{i}=\n{tab}{Expressions[i].Details(indent + 1)}\n";
        }
        str += $"\n{tab}]";
        return str;
    }
}