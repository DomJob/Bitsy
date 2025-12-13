using Bitsy.Lexing;

namespace Bitsy.Parsing;

public class PrefixExpression : Expression
{
    public Token Operation { get; }
    public Expression Expression { get; }

    public PrefixExpression(Token operation, Expression expression)
    {
        Operation = operation;
        Expression = expression;
    }

    public override string ToString() => $"{Operation.Literal}{Expression}";
}