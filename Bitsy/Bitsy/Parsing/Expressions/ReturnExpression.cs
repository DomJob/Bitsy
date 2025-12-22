using Bitsy.Lexing;
using Bitsy.Reading;

namespace Bitsy.Parsing.Expressions;

public class ReturnExpression : Expression
{
    private readonly Token token;

    public ReturnExpression(Token token, Expression expression)
    {
        this.token = token;
        Expression = expression;
    }

    public Expression Expression { get; }

    public override Position Position => token.Position;

    public override string ToString()
    {
        return $"return {Expression}";
    }
}