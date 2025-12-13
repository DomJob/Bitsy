using Bitsy.Lexing;
using Bitsy.Parsing;
using Bitsy.Reading;

namespace Bitsy;

public static class Program
{
    public static void Main()
    {
        while (true)
        {
            Console.Write(">>> ");
            var code = Console.ReadLine() ?? "";

            var reader = new StringCodeReader(code);
            var lexer = new Lexer(reader);
            var parser = new Parser(lexer);

            try
            {
                var expression = parser.ParseExpression();
                Console.WriteLine(expression.ToString());
            }
            catch (ParserException e)
            {
                Console.WriteLine($"Parsing error: {e.Message} - {e.Token}");
            }
        }
    }
}