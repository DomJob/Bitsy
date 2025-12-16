using Bitsy.Lexing;

namespace Bitsy.Parsing.Expressions;

public class UnaryExpression : Expression
{
    public UnaryExpression(Token operation, Expression operand)
    {
        Operation = operation;
        Operand = operand;
    }

    public Token Operation { get; }
    public Expression Operand { get; }

    public override string ToString()
    {
        return $"{Operation.Literal}{Operand}";
    }
}