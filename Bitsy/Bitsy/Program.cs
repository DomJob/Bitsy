using Bitsy.Lexing;
using Bitsy.Reading;

namespace Bitsy;

public static class Program
{
    public static void Main()
    {
        while(true)
        {
            Console.Write(">>> ");
            string code = Console.ReadLine() ?? "";

            var lexer = new Lexer(new StringCodeReader(code));
            while (true)
            {
                var token = lexer.Next();
                Console.WriteLine($"[{token.Type}, \"{token.Literal}\"]");

                if (token.Type == TokenType.End) break;
            }
        }
    }
}