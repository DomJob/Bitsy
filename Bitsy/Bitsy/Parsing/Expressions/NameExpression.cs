using Bitsy.Lexing;

namespace Bitsy.Parsing.Expressions;

public class NameExpression : Expression
{
    public Token Name { get; }

    public List<Expression> Templates { get; set; }

    public NameExpression(Token name)
    {
        Name = name;
        Templates = [];
    }
    
    public override string ToString()
    {
        var templates = Templates.Count == 0 ? "" : "<"+string.Join(", ", Templates)+">";
        
        return Name.Literal + templates;
    }
}