using Bitsy.Lexing;

namespace Bitsy.Parsing.Expressions;

public class NameExpression : Expression
{
    private readonly Token name;

    public NameExpression(Token name)
    {
        this.name = name;
    }

    public override String ToString() => name.Literal;
}