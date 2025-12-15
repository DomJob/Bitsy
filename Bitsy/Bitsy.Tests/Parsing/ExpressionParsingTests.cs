using Bitsy.Lexing;
using Bitsy.Reading;

namespace Bitsy.Parsing;

public class ExpressionParsingTests
{
    private Parser? parser;
    
    [TearDown]
    public void TearDown() => parser = null;
    
    private Expression LoadParser(string code)
    {
        if (parser == null)
        {
            var reader = new StringCodeReader(code);
            var lexer = new Lexer(reader);
            parser = new Parser(lexer);
        }
        return parser.Parse();
    }
}