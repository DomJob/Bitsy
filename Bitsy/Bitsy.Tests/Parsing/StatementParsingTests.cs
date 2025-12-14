using Bitsy.Lexing;
using Bitsy.Parsing.Statements;
using Bitsy.Reading;

namespace Bitsy.Parsing;

public class StatementParsingTests
{
    private Parser? parser = null;
    
    [TearDown]
    public void TearDown() => parser = null;
    
    [Test]
    public void Binding_Simple()
    {
        var code = "a = 1";
        var statement = ParseStatement(code);
        Assert.That(statement, Is.InstanceOf<BindingStatement>());
        Assert.That(statement.ToString(), Is.EqualTo("a = 1"));
    }
    
    [Test]
    public void Binding_Simple_TwoStatements_DifferentLine()
    {
        var code = "a = 1\nb = 0";
        var statement = ParseStatement(code);
        Assert.That(statement, Is.InstanceOf<BindingStatement>());
        Assert.That(statement.ToString(), Is.EqualTo("a = 1"));
        
        var nextStatement = ParseStatement(code);
        Assert.That(nextStatement, Is.InstanceOf<BindingStatement>());
        Assert.That(nextStatement.ToString(), Is.EqualTo("b = 0"));
    }
    
    [Test]
    public void Binding_Simple_TwoStatements_SameLine()
    {
        var code = "a = 1 b = 0";
        var statement = ParseStatement(code);
        Assert.That(statement, Is.InstanceOf<BindingStatement>());
        Assert.That(statement.ToString(), Is.EqualTo("a = 1"));
        
        var nextStatement = ParseStatement(code);
        Assert.That(nextStatement, Is.InstanceOf<BindingStatement>());
        Assert.That(nextStatement.ToString(), Is.EqualTo("b = 0"));
    }
    
    private Statement ParseStatement(string code)
    {
        if (parser == null)
        {
            var reader = new StringCodeReader(code);
            var lexer = new Lexer(reader);
            parser = new Parser(lexer);
        }
        return parser.ParseStatement();
    }
}