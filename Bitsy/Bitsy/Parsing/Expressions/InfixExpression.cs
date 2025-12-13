using Bitsy.Lexing;

namespace Bitsy.Parsing.Expressions;

public class InfixExpression : Expression
{
    public Expression Left { get; set; }
    public Token Operation { get; set; }
    public Expression Right { get; set; }

    public InfixExpression(Expression left, Token operation, Expression right)
    {
        Left = left;
        Operation = operation;
        Right = right;
    }

    public override string ToString()
    {
        return $"({Left} {Operation.Literal} {Right})";
    }
    
    public override string Details(int indent = 0)
    {
        var tab = new string(' ', indent);
        return $"{tab}[Infix: {Operation.Literal}\n{tab+' '}Left: {Left.Details(indent + 1)}\n{tab+' '}Right: {Right.Details(indent + 1)}\n{tab}]";
    }
}