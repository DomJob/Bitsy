using Bitsy.Lexing;

namespace Bitsy.Parsing;

public class IdentifierExpression: Expression
{
    public Token Identifier { get;  }

    public IdentifierExpression(Token identifier)
    {
        Identifier = identifier;
    }

    public override string ToString() => Identifier.Literal;
}