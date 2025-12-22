using Bitsy.Reading;

namespace Bitsy.Parsing.Expressions;

public class FunctionDeclaration : Expression
{
    public FunctionDeclaration(NameExpression name, List<(TypeExpression, NameExpression)> args, List<Expression> body)
    {
        Name = name;
        Args = args;
        Body = body;
    }

    public NameExpression Name { get; }
    public List<(TypeExpression, NameExpression)> Args { get; }
    public List<Expression> Body { get; }

    public override Position Position => Name.Position;

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