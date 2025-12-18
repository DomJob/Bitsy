namespace Bitsy.Parsing.Expressions;

public class TypeDeclaration : Expression
{
    public TypeDeclaration(Expression name, List<(Expression, Expression)> body)
    {
        Name = name;
        Body = body;
        Templates = [];

        Validate();
    }

    public TypeDeclaration(Expression name, List<(Expression, Expression)> body, List<Expression> templates)
    {
        Name = name;
        Body = body;
        Templates = templates;

        Validate();
    }

    public Expression Name { get; }

    public List<(Expression, Expression)> Body { get; }

    public List<Expression> Templates { get; }

    private void Validate()
    {
        if (Name.GetType() != typeof(NameExpression))
            throw new SyntaxError("Invalid type name for declaration", Name.Position);
        foreach (var (bodyType, bodyName) in Body)
        {
            if (bodyType.GetType() != typeof(NameExpression) && bodyType.GetType() != typeof(FunctionTypeExpression))
                throw new SyntaxError("Invalid attribute type in type declaration body", bodyType.Position);
            if (bodyName.GetType() != typeof(NameExpression))
                throw new SyntaxError("Invalid attribute in type declaration body", bodyName.Position);
        }

        foreach (var templateArg in Templates)
            if (templateArg.GetType() != typeof(NameExpression) && templateArg.GetType() != typeof(FunctionTypeExpression))
                throw new SyntaxError("Invalid generic type in type declaration", templateArg.Position);
    }

    public override string ToString()
    {
        var templates = Templates.Count == 0 ? "" : "<" + string.Join(", ", Templates) + ">";
        var body = string.Join(" ", Body.Select(b => $"{b.Item1} {b.Item2}"));
        return $"{Name}{templates} {{ {body} }}";
    }
}