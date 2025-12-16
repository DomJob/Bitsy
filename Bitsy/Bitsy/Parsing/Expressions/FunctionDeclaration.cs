namespace Bitsy.Parsing.Expressions;

public class FunctionDeclaration : Expression
{
    public Expression Name { get; }
    public List<(Expression, Expression)> Args { get; }
    public List<Expression> Body { get; }

    public FunctionDeclaration(Expression name, List<(Expression, Expression)> args, List<Expression> body)
    {
        Name = name;
        Args = args;
        Body = body;
    }

    public override string ToString()
    {
        var str = Name.ToString();
        str += "(";
        str += string.Join(", ", Args.Select(arg => $"{arg.Item1} {arg.Item2}"));
        str += ") { ";
        str += string.Join(" ", Body.Select(e => e.ToString()));
        str += " }";
        return str;
    }
}