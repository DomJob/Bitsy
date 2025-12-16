using System.Linq.Expressions;
using Bitsy.Lexing;

namespace Bitsy.Parsing.Expressions;

public class PrefixExpression : Expression
{
    public Token Operation { get; }
    public Expression Operand { get; }

    public PrefixExpression(Token operation, Expression operand)
    {
        this.Operation = operation;
        this.Operand = operand;
    }

    public override string ToString()
    {
        return $"{Operation.Literal}{Operand}";
    }
}