using Bitsy.Reading;

namespace Bitsy.Parsing;

public abstract class Expression
{
    public abstract Position Position { get; }

    public string Literal => ToString()!;
}