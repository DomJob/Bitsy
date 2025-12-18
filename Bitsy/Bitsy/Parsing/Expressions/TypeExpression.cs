using Bitsy.Lexing;

namespace Bitsy.Parsing.Expressions;

public abstract class TypeExpression : Expression
{
}

public class SimpleTypeExpression : TypeExpression
{
    public SimpleTypeExpression(Token name, List<SimpleTypeExpression>? templates = null)
    {
        Name = name;
        Templates = templates ?? [];
    }

    public Token Name { get; }
    public List<SimpleTypeExpression> Templates { get; }

    public override string ToString()
    {
        return Name.Literal + (Templates.Count == 0 ? "" : $"<{string.Join(", ", Templates)}>");
    }
}

public class UnionTypeExpression : TypeExpression
{
    public UnionTypeExpression(List<TypeExpression> names)
    {
        Names = names;
    }

    public List<TypeExpression> Names { get; }

    public override string ToString()
    {
        return $"({string.Join(", ", Names)})";
    }
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