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
        if (token.Type == TokenType.LeftAngle)
        {
            if (left is not SimpleTypeExpression typeExpr)
                throw new ParserException("Expected type, got " + left.GetType().Name);
            typeExpr.Templates.AddRange(parser.ParseTemplates(true));
            return typeExpr;
        }

        if (token.Type == TokenType.Arrow)
        {
            if (left is not TypeExpression input)
                throw new ParserException("Expected type, got " + left.GetType().Name);
            var right = parser.ParseExpression();
            if (right is not TypeExpression output)
                throw new ParserException("Expected type, got " + left.GetType().Name);
            return new FunctionTypeExpression(input, output);
        }

        if (token.Type == TokenType.Assignment)
        {
            if (left is not NameExpression nameExpr)
                throw new ParserException("Expected identifier, got " + left);
            return new AssignmentExpression(nameExpr, parser.ParseExpression());
        }

        return new BinaryExpression(left, token, parser.ParseExpression(Precedence));
    }
}