using Bitsy.Lexing;

namespace Bitsy.Parsing.Expressions;

public class BinaryExpression : Expression
{
    public BinaryExpression(Expression left, Token operation, Expression right)
    {
        Left = left;
        Operation = operation;
        Right = right;

        if (Operation.Type == TokenType.Assignment && Left.GetType() != typeof(NameExpression))
            throw new SyntaxError("Assignment must target an identifier", Left.Position);
        if (Operation.Type == TokenType.Dot && Right.GetType() != typeof(NameExpression))
            throw new SyntaxError("Object accessor must be an identifier", Right.Position);
        if (Operation.Type == TokenType.As && !Right.GetType().IsSubclassOf(typeof(TypeExpression)))
            throw new SyntaxError("Casting must be to a proper type", Right.Position);
    }

    public Expression Left { get; }
    public Token Operation { get; }
    public Expression Right { get; }

    public override string ToString()
    {
        if (Operation.Type == TokenType.Dot)
            return $"({Left}.{Right})";
        if (Operation.Type == TokenType.Assignment)
            return $"{Left} = {Right}";
        return $"({Left} {Operation.Literal} {Right})";
    }
}