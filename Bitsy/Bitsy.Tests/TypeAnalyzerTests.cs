using Bitsy.Analyzing;
using Bitsy.Analyzing.Types;
using Bitsy.Lexing;
using Bitsy.Parsing;
using Bitsy.Reading;
using Type = Bitsy.Analyzing.Type;

namespace Bitsy;

public class TypeAnalyzerTests
{
    private TestScenario When => new();

    [Test]
    public void BitBinding_Simple()
    {
        When.ReadStatement("a = 1")
            .Then
            .Expression("a").Is<Bit>();
    }

    [Test]
    public void BitBinding_TwoSymbols()
    {
        When.ReadStatement("a = 1").And.ReadStatement("b = 0")
            .Then
            .Expression("a").Is<Bit>()
            .And
            .Expression("b").Is<Bit>();
    }

    [Test]
    public void BitBinding_SymbolResolvesPrevious()
    {
        When.ReadStatement("a = 1").And.ReadStatement("b = a")
            .Then
            .Expression("b").Is<Bit>();
    }

    [Test]
    public void BitBinding_UnknownSymbolThrows()
    {
        When.ReadStatement("a = 1")
            .Then
            .Expression("b").Throws<UnknownSymbolException>();
    }

    [Test]
    public void BindingToBitExpression()
    {
        When.ReadStatement("a = 1 ^ 0")
            .And.ReadStatement("b = a ^ 1")
            .Then.Expression("a").Is<Bit>()
            .And.Expression("b").Is<Bit>();
    }

    [Test]
    public void BindingWithAs_SimpleBit()
    {
        When.ReadStatement("a = 1 as Bit")
            .Then
            .Expression("a").Is<Bit>();
    }

    [Test]
    public void BindingFunctionDefinition_Empty()
    {
        When.ReadStatement("someFunc() { }")
            .Then
            .Expression("someFunc").Is<Function>(f => f is
                { Input: Union { Types.Count: 0 }, Output: Union { Types.Count: 0 } });
    }

    [Test]
    public void BindingFunctionDefinition_ReturnBit()
    {
        When.ReadStatement("someFunc() { return 1 }")
            .Then
            .Expression("someFunc").Is<Function>(f => f is { Input: Union { Types.Count: 0 }, Output: Bit });
    }

    [Test]
    public void BindingFunctionDefinition_WithOneReturnBit()
    {
        When.ReadStatement("someFunc(Bit a) { return 1 }")
            .Then
            .Expression("someFunc").Is<Function>(f => f is { Input: Bit, Output: Bit });
    }

    [Test]
    public void BindingFunctionDefinition_WithTwoArguments()
    {
        When.ReadStatement("someFunc(Bit a, Bit b) { return 1 }")
            .Then
            .Expression("someFunc")
            .Is<Function>(f => f is { Output: Bit })
            .And.Is<Function>(f =>
                f.Input is Union { Types.Count: 2 } u && u.Types[0] == Bit.Instance && u.Types[1] == Bit.Instance);
    }

    [Test]
    public void BindingFunctionThatReturnsArg()
    {
        When.ReadStatement("someFunc(Bit a) { return a }")
            .Then
            .Expression("someFunc").Is<Function>(f => f is { Input: Bit, Output: Bit });
    }

    [Test]
    public void BindingMainFunction_AndCastingAsBit()
    {
        When.ReadStatement("main(Bits input) { return input as Bit }")
            .Then
            .Expression("main").Is<Function>(f => f is { Input: Bits, Output: Bit });
    }

    internal class TestScenario
    {
        private readonly TypeAnalyzer typeAnalyzer;
        private Expression? lastExpression;

        internal TestScenario()
        {
            typeAnalyzer = new TypeAnalyzer();
        }

        public TestScenario And => this;
        public TestScenario Then => this;

        private Expression Parse(string code)
        {
            var reader = new LineReader(code);
            var lexer = new Lexer(reader);
            var parser = new Parser(lexer);
            return parser.Next();
        }

        internal TestScenario ReadStatement(string code)
        {
            typeAnalyzer.LoadExpression(Parse(code));
            return this;
        }

        internal TestScenario Expression(string code)
        {
            lastExpression = Parse(code);
            return this;
        }

        internal TestScenario Is<T>() where T : Type
        {
            var type = typeAnalyzer.ResolveType(lastExpression);
            Console.WriteLine(type);
            Assert.That(type, NUnit.Framework.Is.InstanceOf<T>());
            return this;
        }

        internal TestScenario Is<T>(Func<T, bool> suchThat) where T : Type
        {
            var type = (T)typeAnalyzer.ResolveType(lastExpression);
            Console.WriteLine(type);
            Assert.That(suchThat(type!), NUnit.Framework.Is.True);

            return this;
        }

        internal TestScenario Throws<T>() where T : Exception
        {
            try
            {
                typeAnalyzer.ResolveType(lastExpression);
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