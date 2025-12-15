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