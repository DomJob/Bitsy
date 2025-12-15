using System.Linq.Expressions;
using Bitsy.Lexing;

namespace Bitsy.Parsing.Expressions;

public class PrefixExpression : Expression
{
    private readonly Token operation;
    private readonly Expression operand;

    public PrefixExpression(Token operation, Expression operand)
    {
        this.operation = operation;
        this.operand = operand;
    }

    public override string ToString()
    {
        return $"{operation.Literal}{operand}";
    }
}