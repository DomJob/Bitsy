using Bitsy.Lexing;

namespace Bitsy.Parsing.Parselets;

public interface IPrefixParselet
{
    Expression Parse(Parser parser, Token token);
}