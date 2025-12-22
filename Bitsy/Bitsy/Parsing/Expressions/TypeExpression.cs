using Bitsy.Lexing;
using Bitsy.Reading;

namespace Bitsy.Parsing.Expressions;

public abstract class TypeExpression : Expression
{
}

public class SimpleTypeExpression : TypeExpression
{
    public SimpleTypeExpression(Token name, List<TypeExpression>? templates = null)
    {
        Name = name;
        Templates = templates ?? [];
    }

    public Token Name { get; }
    public List<TypeExpression> Templates { get; }

    public override Position Position => Name.Position;

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
        Position = Names[0].Position;
    }

    public UnionTypeExpression(Position position)
    {
        Names = [];
        Position = position;
    }

    public List<TypeExpression> Names { get; }

    public override Position Position { get; }

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

    public override Position Position => Input.Position;

    public override string ToString()
    {
        return $"({Input}->{Output})";
    }
}