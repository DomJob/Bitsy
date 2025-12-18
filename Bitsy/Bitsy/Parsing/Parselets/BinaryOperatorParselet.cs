using Bitsy.Lexing;
using Bitsy.Parsing.Expressions;

namespace Bitsy.Parsing.Parselets;

public class BinaryOperatorParselet : InfixParselet
{
    public BinaryOperatorParselet(int precedence)
    {
        Precedence = precedence;
    }

    public int Precedence { get; }

    public Expression Parse(Parser parser, Expression left, Token token)
    {
        var right = token.Type == TokenType.As
            ? parser.ParseType(false)
            : parser.ParseExpression(Precedence);
        return new BinaryExpression(left, token, right);
    }
}