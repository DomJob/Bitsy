using Bitsy.Lexing;
using Bitsy.Parsing.Expressions;

namespace Bitsy.Parsing.Parselets;

public class GroupParselet : PrefixParselet
{
    public Expression Parse(Parser parser, Token token)
    {
        if (parser.Match(TokenType.RightParenthesis))
        {
            var input = new UnionTypeExpression(token.Position);
            if (!parser.Match(TokenType.Arrow))
                return input;
            var output = parser.ParseTypeSignature();
            return new FunctionTypeExpression(input, output);
        }

        var expression = parser.ParseExpression();
        if (expression is TypeExpression typeExpr)
        {
            if (parser.Match(TokenType.Arrow))
            {
                var output = parser.ParseTypeSignature();
                return new FunctionTypeExpression(typeExpr, output);
            }

            if (parser.Match(TokenType.Comma))
            {
                List<TypeExpression> types = [typeExpr];
                while (true)
                {
                    types.Add(parser.ParseTypeSignature());
                    if (parser.Match(TokenType.RightParenthesis)) break;
                    parser.Consume(TokenType.Comma);
                }

                return new UnionTypeExpression(types);
            }
        }
        else
        {
            parser.Consume(TokenType.RightParenthesis);
        }

        return expression;
    }

    public int Precedence { get; }
}