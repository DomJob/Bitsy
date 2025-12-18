using Bitsy.Lexing;

namespace Bitsy.Parsing.Expressions;

public abstract class TypeExpression : Expression
{
    
}

public class SimpleTypeExpression : TypeExpression
{
    public Token Name { get; }
    public List<SimpleTypeExpression> Templates { get; }

    public SimpleTypeExpression(Token name, List<SimpleTypeExpression>? templates = null)
    {
        Name = name;
        Templates = templates ?? [];
    }
    
    public override String ToString() => Name.Literal + (Templates.Count == 0 ? "" : $"<{string.Join(", ", Templates)}>");
}

public class UnionTypeExpression : TypeExpression
{
    public List<TypeExpression> Names { get; }

    public UnionTypeExpression(List<TypeExpression> names)
    {
        Names = names;
    }
    
    public override String ToString() => $"({string.Join(", ", Names)})";
}

public class FunctionTypeExpression : TypeExpression
{
    public FunctionTypeExpression(TypeExpression input, TypeExpression output)
    {
        Input = input;
        Output = output;
    }

    public TypeExpression Input { get; }

    public TypeExpression Output { get; }

    public override string ToString()
    {
        return $"({Input}->{Output})";
    }
}