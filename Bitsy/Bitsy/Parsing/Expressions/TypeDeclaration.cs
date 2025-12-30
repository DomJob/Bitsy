using Bitsy.Reading;

namespace Bitsy.Parsing.Expressions;

public class TypeDeclaration : Expression
{
    public TypeDeclaration(SimpleTypeExpression name, List<(TypeExpression, NameExpression)> body)
    {
        Name = name;
        Body = body;
    }

    public SimpleTypeExpression Name { get; }
    
    public List<TypeExpression> Templates => Name.Templates; 
    
    public List<(TypeExpression, NameExpression)> Body { get; }

    public override Position Position => Name.Position;


    public override string ToString()
    {
        var body = string.Join(" ", Body.Select(b => $"{b.Item1} {b.Item2}"));
        return $"{Name} {{ {body} }}";
    }
}