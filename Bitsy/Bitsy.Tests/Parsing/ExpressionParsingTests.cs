using Bitsy.Lexing;
using Bitsy.Parsing.Expressions;
using Bitsy.Reading;

namespace Bitsy.Parsing;

public class ExpressionParsingTests
{
    private Expression expression;
    
    [Test]
    public void Identifier_Simple()
    {
        ParseExpression("abc");
        
        Verify<NameExpression>("abc");
    }
    
    [Test]
    public void Prefix_NotExpression()
    {
        ParseExpression("~abc");
        
        Verify<PrefixExpression>("~abc");
    }

    [Test]
    public void Infix_AndOperation()
    {
        ParseExpression("a & b");
        
        Verify<OperationExpression>("(a & b)");
    }
    
    [Test]
    public void Infix_OrOperation()
    {
        ParseExpression("a | b");
        
        Verify<OperationExpression>("(a | b)");
    }
    
    [Test]
    public void Infix_XorOperation()
    {
        ParseExpression("a ^ b");
        
        Verify<OperationExpression>("(a ^ b)");
    }
    
    [Test]
    public void NegateWithAndOperation()
    {
        ParseExpression("~a & b");
        
        Verify<OperationExpression>("(~a & b)");
    }
    
    [Test]
    public void NegateAndOperation()
    {
        ParseExpression("~(a & b)");
        
        Verify<PrefixExpression>("~(a & b)");
    }

    [Test]
    public void AndOperationsLeftAssociative()
    {
        ParseExpression("a & b & c");
        
        Verify<OperationExpression>("((a & b) & c)");
    }
    
    [Test]
    public void AndOrOperationsProperOrder()
    {
        ParseExpression("a | b & c");
        
        Verify<OperationExpression>("(a | (b & c))");
    }
    
    [Test]
    public void AndXOrOperationsProperOrder()
    {
        ParseExpression("a ^ b & c");
        
        Verify<OperationExpression>("(a ^ (b & c))");
    }
    
    private void Verify<T>(string value) where T : Expression
    {
        Assert.That(expression.ToString(), Is.EqualTo(value));
        Assert.That(expression, Is.TypeOf<T>());
    }
    
    private void Verify(string value) => Verify<Expression>(value);
    
    private Expression ParseExpression(string code)
    {
        var reader = new StringCodeReader(code);
        var lexer = new Lexer(reader);
        var parser = new Parser(lexer);
        expression = parser.ParseExpression();
        return expression;
    }
}