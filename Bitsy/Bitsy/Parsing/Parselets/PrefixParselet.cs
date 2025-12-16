using Bitsy.Lexing;

namespace Bitsy.Parsing.Parselets;

public interface PrefixParselet
{
    public int Precedence { get; }
    
    Expression Parse(Parser parser, Token token);
}