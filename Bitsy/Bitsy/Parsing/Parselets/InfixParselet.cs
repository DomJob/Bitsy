using Bitsy.Lexing;

namespace Bitsy.Parsing.Parselets;

public interface InfixParselet
{
    Expression Parse(Parser parser, Expression left, Token token);
    
    int Precedence { get; }
}