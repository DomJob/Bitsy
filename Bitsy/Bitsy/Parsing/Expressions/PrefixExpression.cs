using Bitsy.Lexing;

namespace Bitsy.Parsing.Expressions;

public class PrefixExpression : Expression
{
    public PrefixExpression(Token operation, Expression expression)
    {
        Operation = operation;
        Expression = expression;
    }

    public Token Operation { get; }
    public Expression Expression { get; }

    public override string ToString()
    {
        return $"{Operation.Literal}{Expression}";
    }

    public override string Details(int indent = 0)
    {
        var tab = new string(' ', indent);
        return $"{tab}[Prefix: {Operation.Literal}\n{tab}{Expression.Details(indent + 1)}\n{tab}]";
    }
}