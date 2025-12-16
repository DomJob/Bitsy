using Bitsy.Lexing;

namespace Bitsy.Parsing.Parselets;

public interface InfixParselet
{
    int Precedence { get; }
    Expression Parse(Parser parser, Expression left, Token token);
}