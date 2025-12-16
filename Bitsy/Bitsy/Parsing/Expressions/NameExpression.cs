using Bitsy.Lexing;

namespace Bitsy.Parsing.Expressions;

public class NameExpression : Expression
{
    public NameExpression(Token name)
    {
        Name = name;
    }

    public Token Name { get; }

    public override string ToString()
    {
        return Name.Literal;
    }
}