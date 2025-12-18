using Bitsy.Evaluating;
using Bitsy.Lexing;
using Bitsy.Parsing;
using Bitsy.Parsing.Expressions;
using Bitsy.Reading;

namespace Bitsy;

public class Program
{
    public List<Bit> RunCode(string code, List<Bit>? input = null)
    {
        if (input == null) input = [];

        var reader = new LineReader(code);
        var lexer = new Lexer(reader);
        var parser = new Parser(lexer);

        var environment = new Evaluator();

        environment.Load(parser);

        var args = input.Select(b => b == Bit.Zero ? NameExpression.Zero : NameExpression.One).ToList();

        var mainCall = new CallExpression(NameExpression.Main, args);

        var result = environment.Evaluate(mainCall);
        return result.ToBits();
    }

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