using Bitsy.Lexing;

namespace Bitsy.Parsing.Expressions;

public class IdentifierExpression : Expression
{
    public IdentifierExpression(Token identifier)
    {
        Identifier = identifier;
    }

    public Token Identifier { get; }

    public override string ToString()
    {
        return Identifier.Literal;
    }
}