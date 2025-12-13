using Bitsy.Lexing;

namespace Bitsy.Parsing.Expressions;

public class IdentifierExpression: Expression
{
    public Token Identifier { get;  }

    public IdentifierExpression(Token identifier)
    {
        Identifier = identifier;
    }

    public override string ToString() => Identifier.Literal;
    
    public override string Details(int indent = 0)
    {
        return $"Id({Identifier.Literal})";
    }
}