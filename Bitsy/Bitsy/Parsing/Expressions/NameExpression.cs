using Bitsy.Lexing;
using Bitsy.Reading;

namespace Bitsy.Parsing.Expressions;

public class NameExpression : Expression
{
    public NameExpression(Token name)
    {
        Name = name;
    }

    public Token Name { get; }

    public new Position Position => Name.Position;
    
    public SimpleTypeExpression ToType(List<TypeExpression>? templates = null)
    {
        return new SimpleTypeExpression(Name, templates??[]);
    }

    public override string ToString() => Name.Literal;
}