namespace Bitsy.Parsing.Expressions;

public class TypeDeclaration : Expression
{
    public Expression Name { get; }
    
    public List<(Expression, Expression)> Body { get; }
    
    public List<Expression> Templates { get; }

    public TypeDeclaration(Expression name, List<(Expression, Expression)> body)
    {
        Name = name;
        Body = body;
        Templates = [];
    }

    public TypeDeclaration(Expression name, List<(Expression, Expression)> body, List<Expression> templates)
    {
        Name = name;
        Body = body;
        Templates = templates;
    }

    public override string ToString()
    {
        var templates = Templates.Count == 0 ? "" : "<"+string.Join(", ", Templates)+">";
        var body = string.Join(" ", Body.Select(b => $"{b.Item1} {b.Item2}"));
        return $"{Name}{templates} {{ {body} }}";
    }
}