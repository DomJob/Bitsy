using Bitsy.Lexing;
using Bitsy.Parsing;
using Bitsy.Reading;

namespace Bitsy;

public class Program
{
    public static void Main()
    {
        while (true)
        {
            Console.Write(">>> ");
            var code = Console.ReadLine() ?? "";

            var reader = new LineReader(code);
            var lexer = new Lexer(reader);
            var parser = new Parser(lexer);

            try
            {
                var expression = parser.ParseStatement();
                Console.WriteLine(expression.GetType() + " : " + expression);
            }
            catch (ParserException e)
            {
                Console.WriteLine($"Parsing error: {e.Message} - {e.Token}");
                Console.WriteLine(e.StackTrace);
            }
            catch (SyntaxError e)
            {
                Console.WriteLine($"Syntax error: {e.Message} - {e.Position}");
            }
        }
    }
}