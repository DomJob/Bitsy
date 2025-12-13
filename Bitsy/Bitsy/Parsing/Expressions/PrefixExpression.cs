using Bitsy.Lexing;

namespace Bitsy.Parsing.Expressions;

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
    
    public override string Details(int indent = 0)
    {
        var tab = new string(' ', indent);
        return $"{tab}[Prefix: {Operation.Literal}\n{tab}{Expression.Details(indent + 1)}\n{tab}]";
    }
}