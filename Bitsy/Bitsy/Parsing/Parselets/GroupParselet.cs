using Bitsy.Lexing;

namespace Bitsy.Parsing.Parselets;

public class GroupParselet : PrefixParselet
{
    public Expression Parse(Parser parser, Token token)
    {
        var expression = parser.ParseExpression(Precedence);
        parser.Consume(TokenType.RightParenthesis);
        return expression;
    }

    public int Precedence { get; }
}