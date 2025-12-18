using Bitsy.Evaluating;
using Bitsy.Lexing;
using Bitsy.Parsing;
using Bitsy.Parsing.Expressions;
using Bitsy.Reading;
using BinaryExpression = System.Linq.Expressions.BinaryExpression;

namespace Bitsy;

public class EvaluatorTests
{
    [Test]
    public void Evaluate_SimpleBinding()
    {
        var evaluator = new Evaluator();

        var assignmentExpr = ParseExpression("a = 1");
        
        evaluator.LoadExpression(assignmentExpr);
        
        var val = evaluator.Evaluate(ParseExpression("a"));
        Assert.That(val.ToString(), Is.EqualTo("a :: Bit = 1"));
        // assert that a has type Bit
        // assert that a has value 1
    }
    
    [Test]
    public void Evaluate_()
    {
        var evaluator = new Evaluator();

        
        evaluator.LoadExpression(ParseExpression("Type {Bit attr}"));
        evaluator.LoadExpression(ParseExpression("obj = {attr: 1} as Type"));
        
        var val1 = evaluator.Evaluate(ParseExpression("obj"));
        // assert that obj has type 'Type'?
        var val2 = evaluator.Evaluate(ParseExpression("obj.attr"));
        // assert that a has value 1
    }
    
    private Expression ParseExpression(string code)
    {
        var reader = new StringCodeReader(code);
        var lexer = new Lexer(reader);
        var parser = new Parser(lexer);
        return parser.ParseExpression();
    }
}