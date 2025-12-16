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
        if (Operation.Type == TokenType.Return)
        {
            return $"return {Operand}";
        }
        return $"{Operation.Literal}{Operand}";
    }
}