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
        if (token.Type == TokenType.Arrow)
        {
            if (left is NameExpression name)
            {
                var type = new SimpleTypeExpression(name.Name, []);
                return new FunctionTypeExpression(type, parser.ParseType());
            }
            else if (left is TypeExpression type)
            {
                return new FunctionTypeExpression(type, parser.ParseType());
            }

            throw new ParserException("Expected type, got " + left.GetType().Name);
        }

        if (token.Type == TokenType.LeftAngle)
        {
            if (left is not NameExpression nameExpression)
                throw new ParserException("Expected name, got " + left.GetType().Name);

            return new SimpleTypeExpression(nameExpression.Name, parser.ParseTemplates(true));
        }

        var right = token.Type == TokenType.As
            ? parser.ParseType(false)
            : parser.ParseExpression(Precedence);
        return new BinaryExpression(left, token, right);
    }
}