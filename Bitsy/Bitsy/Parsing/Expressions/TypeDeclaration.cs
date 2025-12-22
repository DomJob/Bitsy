namespace Bitsy.Parsing.Expressions;

public class TypeDeclaration : Expression
{
    public TypeDeclaration(TypeExpression name, List<(TypeExpression, NameExpression)> body)
    {
        Name = name;
        Body = body;
    }

    public TypeExpression Name { get; }

    public List<(TypeExpression, NameExpression)> Body { get; }


    public override string ToString()
    {
        var body = string.Join(" ", Body.Select(b => $"{b.Item1} {b.Item2}"));
        return $"{Name} {{ {body} }}";
    }
}