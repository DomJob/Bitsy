using Bitsy.Lexing;
using Bitsy.Reading;

namespace Bitsy.Parsing.Expressions;

public class NameExpression : Expression
{
    public static readonly Expression Main = new NameExpression(Token.Main);
    public static readonly Expression Zero = new NameExpression(Token.Zero);
    public static readonly Expression One = new NameExpression(Token.One);
    
    public NameExpression(Token name)
    {
        Name = name;
        Templates = [];
    }

    public Token Name { get; }

    public List<Expression> Templates { get; set; }

    public new Position Position => Name.Position;

    public override string ToString()
    {
        var templates = Templates.Count == 0 ? "" : "<" + string.Join(", ", Templates) + ">";

        return Name.Literal + templates;
    }
}