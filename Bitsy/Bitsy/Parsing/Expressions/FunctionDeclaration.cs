namespace Bitsy.Parsing.Expressions;

public class FunctionDeclaration : Expression
{
    public FunctionDeclaration(Expression name, List<(Expression, Expression)> args, List<Expression> body)
    {
        Name = name;
        Args = args;
        Body = body;

        if (name.GetType() != typeof(NameExpression))
            throw new SyntaxError("Invalid function name", name.Position);

        foreach (var (type, argname) in args)
        {
            if (type.GetType() != typeof(NameExpression) && type.GetType() != typeof(TypeExpression))
                throw new SyntaxError("Invalid function argument type", type.Position);
            if (argname.GetType() != typeof(NameExpression))
                throw new SyntaxError("Function argument must be a name", argname.Position);
        }
    }

    public Expression Name { get; }
    public List<(Expression, Expression)> Args { get; }
    public List<Expression> Body { get; }

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