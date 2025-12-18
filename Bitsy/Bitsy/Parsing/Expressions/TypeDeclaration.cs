namespace Bitsy.Parsing.Expressions;

public class TypeDeclaration : Expression
{
    public TypeDeclaration(Expression name, List<TypeExpression> templates, List<(TypeExpression, NameExpression)> body)
    {
        Name = name;
        Templates = templates;
        Body = body;
    }

    public Expression Name { get; }

    public List<TypeExpression> Templates { get; }

    public List<(TypeExpression, NameExpression)> Body { get; }


    public override string ToString()
    {
        var templates = Templates.Count == 0 ? "" : "<" + string.Join(", ", Templates) + ">";
        var body = string.Join(" ", Body.Select(b => $"{b.Item1} {b.Item2}"));
        return $"{Name}{templates} {{ {body} }}";
    }
}