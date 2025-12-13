namespace Bitsy.Parsing.Expressions;

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
    
    public override string Details(int indent = 0)
    {
        var tab = new string(' ', indent);
        var str = $"{tab}[Call, target =\n";
        str += Target.Details(indent + 1) + "\n";
        for(var i = 0; i < Arguments.Count; i++)
        {
            str += $"{tab}Arg {i}=\n{Arguments[i].Details(indent + 1)}\n";
        }
        str += $"\n{tab}]";
        return str;
    }
}