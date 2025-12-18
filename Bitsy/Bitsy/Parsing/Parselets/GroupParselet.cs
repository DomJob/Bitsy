using Bitsy.Lexing;
using Bitsy.Parsing.Expressions;

namespace Bitsy.Parsing.Parselets;

public class GroupParselet : PrefixParselet
{
    public Expression Parse(Parser parser, Token token)
    {
        if (parser.Match(TokenType.RightParenthesis))
        {
            parser.Consume(TokenType.Arrow);
            var input = new UnionTypeExpression([]);
            var output = parser.ParseTypeSignature();
            return new FunctionTypeExpression(input, output);
        }

        var expression = parser.ParseExpression();
        if (expression is NameExpression nameExpr)
        {
            if (parser.Match(TokenType.Arrow))
            {
                var input = new SimpleTypeExpression(nameExpr.Name, []);
                var output = parser.ParseTypeSignature();
                return new FunctionTypeExpression(input, output);
            }

            if (parser.Match(TokenType.Comma))
            {
                List<TypeExpression> types = [new SimpleTypeExpression(nameExpr.Name, [])];
                while (true)
                {
                    types.Add(parser.ParseTypeSignature());
                    if (parser.Match(TokenType.RightParenthesis)) break;
                    parser.Consume(TokenType.Comma);
                }

                return new UnionTypeExpression(types);
            }
        }

        parser.Consume(TokenType.RightParenthesis);
        return expression;
    }

    public int Precedence { get; }
}