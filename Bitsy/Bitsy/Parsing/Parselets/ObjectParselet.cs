using Bitsy.Lexing;
using Bitsy.Parsing.Expressions;

namespace Bitsy.Parsing.Parselets;

public class ObjectParselet : PrefixParselet
{
    public int Precedence => BindingPower.Object;

    public Expression Parse(Parser parser, Token token)
    {
        var expr = parser.ParseExpression(Precedence);

        if (parser.Match(TokenType.Colon)) // Explicit
        {
            if (parser.Match(TokenType.RightBrace))
                throw new ParserException("Expected expression, can't have empty object", token);
            
            var key = expr;
            var value = parser.ParseExpression();
            List<(Expression, Expression)> body = [(key, value)];

            while (parser.Match(TokenType.Comma))
            {
                key = parser.ParseExpression();
                parser.Consume(TokenType.Colon);
                value = parser.ParseExpression();
                body.Add((key, value));
            }
            parser.Consume(TokenType.RightBrace);
            return new ExplicitObjectExpression(body);
        }
        else // Implicit
        {
            parser.Consume(TokenType.Comma);
            // TODO
            return null;
        }
    }
}