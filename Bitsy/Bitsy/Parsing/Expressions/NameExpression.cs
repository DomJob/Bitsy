using Bitsy.Lexing;

namespace Bitsy.Parsing.Expressions;

public class NameExpression : Expression
{
    public Token Name { get; }

    public NameExpression(Token name)
    {
        Name = name;
    }

    public override String ToString() => Name.Literal;
}