using Bitsy.Analyzing;
using Bitsy.Analyzing.Types;
using Bitsy.Lexing;
using Bitsy.Parsing;
using Bitsy.Reading;
using Type = Bitsy.Analyzing.Type;

namespace Bitsy;

public class AnalyzerTests
{
    private TestScenario When => new TestScenario();
    
    [Test]
    public void BitBinding_Simple()
    {
        When.ReadExpression("a = 1")
            .Then
            .Expression("a").Is<Bit>();
    }
    
    [Test]
    public void BitBinding_TwoSymbols()
    {
        When.ReadExpression("a = 1").And.ReadExpression("b = 0")
            .Then
            .Expression("a").Is<Bit>()
            .And
            .Expression("b").Is<Bit>();
    }
    
    [Test]
    public void BitBinding_SymbolResolvesPrevious()
    {
        When.ReadExpression("a = 1").And.ReadExpression("b = a")
            .Then
            .Expression("b").Is<Bit>();
    }
    
    [Test]
    public void BitBinding_UnknownSymbolThrows()
    {
        When.ReadExpression("a = 1")
            .Then
            .Expression("b").Throws<TypeException>();
    }

    internal class TestScenario
    {
        private Analyzer analyzer;
        private Expression? lastExpression;
        
        internal TestScenario() => analyzer = new Analyzer();

        public TestScenario And => this;
        public TestScenario Then => this;

        private Expression Parse(string code)
        {
            var reader = new LineReader(code);
            var lexer = new Lexer(reader);
            var parser = new Parser(lexer);
            return parser.ParseStatement();
        }
        
        internal TestScenario ReadExpression(string code)
        {
            analyzer.LoadExpression(Parse(code));
            return this;
        }

        internal TestScenario Expression(string code)
        {
            lastExpression = Parse(code);
            return this;
        }

        internal TestScenario Is<T>() where T : Type
        {
            var type = analyzer.ResolveType(lastExpression);
            Assert.That(type, NUnit.Framework.Is.InstanceOf<T>());
            return this;
        }

        internal TestScenario With(Func<Type, bool> predicate)
        {
            var type = analyzer.ResolveType(lastExpression);
            Assert.That(predicate(type!), NUnit.Framework.Is.True);

            return this;
        }

        internal TestScenario Throws<T>() where T : Exception
        {
            try
            {
                analyzer.ResolveType(lastExpression);
                Assert.Fail();
            }
            catch (T)
            {
                Assert.Pass();
            }

            return this;
        }
    }
}