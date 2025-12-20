using Bitsy.Lexing;
using Bitsy.Reading;

namespace Bitsy.Parsing.Expressions;

public class NameExpression : Expression
{
    public NameExpression(Token name)
    {
        Name = name;
    }

    public NameExpression(string name)
    {
        Name = new Token(TokenType.Identifier, new Position(), name);
    }

    public Token Name { get; }

    public new Position Position => Name.Position;
    
    public SimpleTypeExpression ToType(List<TypeExpression>? templates = null)
    {
        return new SimpleTypeExpression(Name, templates??[]);
    }

    public override string ToString() => Name.Literal;
}