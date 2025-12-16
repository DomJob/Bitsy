using Bitsy.Lexing;

namespace Bitsy.Parsing.Parselets;

public interface PrefixParselet
{
    Expression Parse(Parser parser, Token token);
}