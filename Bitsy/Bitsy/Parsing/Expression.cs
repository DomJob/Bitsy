using Bitsy.Reading;

namespace Bitsy.Parsing;

public abstract class Expression
{
    public Position Position { get; set; }

    public string Literal => ToString()!;
}