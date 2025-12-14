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
                var statement = parser.ParseStatement();
                Console.WriteLine(statement.GetType().Name);
                Console.WriteLine(statement.ToString());
            }
            catch (ParserException e)
            {
                Console.WriteLine($"Parsing error: {e.Message} - {e.Token}");
            }
        }
    }
}