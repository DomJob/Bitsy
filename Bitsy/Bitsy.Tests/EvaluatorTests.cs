using Bitsy.Evaluating;
using Bitsy.Lexing;
using Bitsy.Parsing;
using Bitsy.Parsing.Expressions;
using Bitsy.Reading;
using BinaryExpression = System.Linq.Expressions.BinaryExpression;
using Environment = Bitsy.Evaluating.Environment;

namespace Bitsy;

public class EvaluatorTests
{
    [Test]
    public void Evaluate_SimpleBinding()
    {
        var env = new Environment();
        var evaluator = new Evaluator(env);

        var assignmentExpr = ParseExpression("a = 1");
        
        
    }
    
    private Expression ParseExpression(string code)
    {
        var reader = new StringCodeReader(code);
        var lexer = new Lexer(reader);
        var parser = new Parser(lexer);
        return parser.ParseExpression();
    }
}