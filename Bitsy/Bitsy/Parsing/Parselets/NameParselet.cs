using Bitsy.Lexing;
using Bitsy.Parsing.Expressions;

namespace Bitsy.Parsing.Parselets;

public class NameParselet : PrefixParselet
{
    public Expression Parse(Parser parser, Token token)
    {
        return new NameExpression(token);
    }

    public int Precedence { get; } = 0;
}