using Bitsy.Analyzing;
using Bitsy.Analyzing.Types;
using Bitsy.Lexing;
using Bitsy.Parsing;
using Bitsy.Reading;
using Type = Bitsy.Analyzing.Type;

namespace Bitsy;

public class TypeEnvironmentTests
{
    private TestScenario When => new();

    [Test]
    public void BitBinding_Simple()
    {
        When.ReadStatement("a = 1")
            .Then
            .Expression("a").HasType<Bit>();
    }

    [Test]
    public void BitBinding_TwoSymbols()
    {
        When.ReadStatement("a = 1").And.ReadStatement("b = 0")
            .Then
            .Expression("a").HasType<Bit>()
            .And
            .Expression("b").HasType<Bit>();
    }

    [Test]
    public void BitBinding_CantRebindSameSymbol()
    {
        When.ReadStatement("a = 1")
            .And.ReadingStatementThrows<SymbolAlreadyDefinedException>("a = 0");
    }

    [Test]
    public void BitBinding_SymbolResolvesPrevious()
    {
        When.ReadStatement("a = 1").And.ReadStatement("b = a")
            .Then
            .Expression("b").HasType<Bit>();
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
            .Then.Expression("a").HasType<Bit>()
            .And.Expression("b").HasType<Bit>();
    }

    [Test]
    public void BindingToUnaryExpression()
    {
        When.ReadStatement("a = 1")
            .And.ReadStatement("b = ~a")
            .Then.Expression("a").HasType<Bit>()
            .And.Expression("b").HasType<Bit>();
    }

    [Test]
    public void BindingWithAs_SimpleBit()
    {
        When.ReadStatement("a = 1 as Bit")
            .Then
            .Expression("a").HasType<Bit>();
    }

    [Test]
    public void BindingWithAs_SimpleBits()
    {
        When.ReadStatement("a = 1 as Bits")
            .Then
            .Expression("a").HasType<Bits>();
    }

    [Test]
    public void BindingFunctionDefinition_Empty()
    {
        When.ReadStatement("someFunc() { }")
            .Then
            .Expression("someFunc").HasType<Function>("(()->())");
    }

    [Test]
    public void BindingFunctionDefinition_ReturnBit()
    {
        When.ReadStatement("someFunc() { return 1 }")
            .Then
            .Expression("someFunc").HasType<Function>("(()->Bit)");
    }

    [Test]
    public void BindingFunctionDefinition_WithOneReturnBit()
    {
        When.ReadStatement("someFunc(Bit a) { return 1 }")
            .Then
            .Expression("someFunc").HasType<Function>("(Bit->Bit)");
    }

    [Test]
    public void BindingFunctionDefinition_WithTwoArguments()
    {
        When.ReadStatement("someFunc(Bit a, Bit b) { return 1 }")
            .Then
            .Expression("someFunc")
            .HasType<Function>("((Bit, Bit)->Bit)");
    }

    [Test]
    public void BindingFunctionDefinition_ThenCallingIt()
    {
        When.ReadStatement("someFunc() { return 1 }")
            .Then
            .Expression("someFunc()").HasType<Bit>();
    }

    [Test]
    public void BindingFunctionDefinition_TwoArgs_ThenCallingIt()
    {
        When.ReadStatement("someFunc(Bit a, Bit b) { return 1 }")
            .Then
            .Expression("someFunc(1,0)")
            .HasType<Bit>();
    }

    [Test]
    public void BindingFunctionDefinition_ReturnFunc_ThenCallingItTwice()
    {
        When.ReadStatement("someFunc() { innerFunc() { return 1} return innerFunc }")
            .Then
            .Expression("someFunc").HasType<Function>("(()->(()->Bit))")
            .And.Expression("(someFunc())()").HasType<Bit>();
    }

    [Test]
    public void BindingFunctionThatReturnsArg()
    {
        When.ReadStatement("someFunc(Bit a) { return a }")
            .Then
            .Expression("someFunc").HasType<Function>("(Bit->Bit)");
    }

    [Test]
    public void BindingFunctionThatReturnsSymbolDefinedInFunc()
    {
        When.ReadStatement("someFunc(Bit a) { b = a ^ 0 return b }")
            .Then
            .Expression("someFunc").HasType<Function>("(Bit->Bit)");
    }

    [Test]
    public void BindingFunctionThatReturnsSymbolDefinedOutOfFunc()
    {
        When.ReadStatement("b = 1")
            .And.ReadStatement("someFunc(Bit a) { return a ^ b }")
            .Then
            .Expression("someFunc").HasType<Function>("(Bit->Bit)");
    }

    [Test]
    public void BindingFunctionThatReturnsOutsideValue()
    {
        When.ReadStatement("b = 1")
            .And.ReadStatement("someFunc(Bit a) { return a ^ b }")
            .Then
            .Expression("someFunc").HasType<Function>("(Bit->Bit)");
    }

    [Test]
    public void BindingMainFunction_AndCastingAsBit()
    {
        When.ReadStatement("main(Bits input) { return input as Bit }")
            .Then
            .Expression("main").HasType<Function>("(Bits->Bit)");
    }

    [Test]
    public void DefiningCustomType_AndUsingIt_1()
    {
        When.ReadStatement("""
                           SomeType {
                            Bit b1
                            Bit b2
                           }
                           """)
            .Then.Expression("{0,1} as SomeType").HasType<Struct>("SomeType");
    }

    [Test]
    public void DefiningCustomType_WithoutUsingIt_IsInterpretedAsBits()
    {
        When.ReadStatement("""
                           SomeType {
                            Bit b1
                            Bit b2
                           }
                           """)
            .Then.Expression("{0,1}").HasType<Bits>("Bits");
    }

    [Test]
    public void DefiningCustomType_WithoutUsingIt_Throws()
    {
        When.ReadStatement("""
                           SomeType {
                            Bit b1
                            Bit b2
                           }
                           """)
            .And.ReadingStatementThrows<SymbolAlreadyDefinedException>
            ("""
             SomeType {
              Bit b1
              Bit b2
             }
             """);
    }

    [Test]
    public void DefiningCustomType_UsingItWithoutCasting_IsInferred()
    {
        When.ReadStatement("""
                           SomeType {
                            Bit b1
                            Bit b2
                           }
                           """)
            .Then.Expression("{b1: 0, b2: 1}").HasType<Struct>("SomeType");
    }

    [Test]
    public void DefiningCustomType_AndUsingIt_2()
    {
        When.ReadStatement("""
                           SomeType {
                            Bit b1
                            Bit b2
                           }
                           """)
            .Then.Expression("{b1: 0, b2: 1} as SomeType").HasType<Struct>("SomeType");
    }

    [Test]
    public void DefiningCustomType_AndUsingIt_WrongTypeField()
    {
        When.ReadStatement("""
                           SomeType {
                            Bit b1
                            Bit b2
                           }
                           """)
            .Then.ReadStatement("b = {b1: 0, b2: 1} as SomeType")
            .Then.Expression("{b1: 0, b2: b} as SomeType").Throws<WrongTypeException>();
    }

    [Test]
    public void DefiningCustomType_AndUsingIt_WrongFieldName()
    {
        When.ReadStatement("""
                           SomeType {
                            Bit b1
                            Bit b2
                           }
                           """)
            .Then.Expression("{b1: 0, zz: 1} as SomeType").Throws<WrongTypeException>();
    }

    [Test]
    public void DefiningCustomType_AndUsingIt_InFunction()
    {
        When.ReadStatement("""
                           SomeType {
                            Bit b1
                            Bit b2
                           }
                           """)
            .And.ReadStatement("someFunc(SomeType obj) { return 1 }")
            .Then.Expression("someFunc").HasType<Function>("(SomeType->Bit)");
    }

    [Test]
    public void DefiningCustomType_AndUsingIt_InFunction2()
    {
        When.ReadStatement("""
                           SomeType {
                            Bit b1
                            Bit b2
                           }
                           """)
            .And.ReadStatement("someFunc(SomeType obj) { return {0,1} as SomeType }")
            .Then.Expression("someFunc").HasType<Function>("(SomeType->SomeType)");
    }

    [Test]
    public void DefiningCustomType_WithOtherCustomTypeInside()
    {
        When.ReadStatement("""
                           SomeType {
                            Bit b1
                            Bit b2
                           }
                           """)
            .And.ReadStatement("""
                               SomeType2 {
                                Bit c1
                                Bit c2
                                SomeType c3
                               }
                               """)
            .Then.Expression("1 as SomeType2").HasType<Struct>("SomeType2");
    }

    [Test]
    public void DefiningRecursiveType()
    {
        When.ReadStatement("""
                           SomeType {
                            Bit b1
                            SomeType more
                           }
                           """)
            .Then.Expression("1 as SomeType").HasType<Struct>("SomeType");
    }

    [Test]
    public void ConditionalTyping_FirstExpressionHasToBeBit()
    {
        When.ReadStatement("""
                           SomeType {
                            Bit b1
                            SomeType more
                           }
                           """)
            .And.ReadStatement("cond = {1,0} as SomeType")
            .Then.Expression("cond ? 1 : 0").Throws<WrongTypeException>();
    }

    [Test]
    public void ConditionalTyping_BothBranchHaveToHaveSameType()
    {
        When.ReadStatement("""
                           SomeType {
                            Bit b1
                            Bit b2
                           }
                           """)
            .And.ReadStatement("cond = 1")
            .And.ReadStatement("a = {1,0} as SomeType")
            .And.ReadStatement("b = 1")
            .Then.Expression("cond ? a : b").Throws<WrongTypeException>();
    }

    [Test]
    public void ConditionalTyping_HappyPathResultingTypeIsBranchesType()
    {
        When.ReadStatement("""
                           SomeType {
                            Bit b1
                            Bit b2
                           }
                           """)
            .And.ReadStatement("cond = 1")
            .And.ReadStatement("a = {1,0} as SomeType")
            .And.ReadStatement("b = {b1: 1, b2: 1} as SomeType")
            .Then.Expression("cond ? a : b").HasType<Struct>("SomeType");
    }

    internal class TestScenario
    {
        private readonly TypeEnvironment typeEnvironment;
        private Expression? lastExpression;

        internal TestScenario()
        {
            typeEnvironment = new TypeEnvironment();
        }

        public TestScenario And => this;
        public TestScenario Then => this;

        private Expression Parse(string code)
        {
            var reader = new LineReader(code);
            var lexer = new Lexer(reader);
            var parser = new Parser(lexer);
            return parser.ParseExpression();
        }

        internal TestScenario ReadStatement(string code)
        {
            typeEnvironment.ReadStatement(Parse(code));
            return this;
        }

        internal TestScenario ReadingStatementThrows<T>(string code) where T : Exception
        {
            try
            {
                typeEnvironment.ReadStatement(Parse(code));
                Assert.Fail();
            }
            catch (T e)
            {
                Assert.That(e, Is.TypeOf(typeof(T)));
            }

            return this;
        }

        internal TestScenario Expression(string code)
        {
            lastExpression = Parse(code);
            return this;
        }

        internal TestScenario HasType<T>() where T : Type
        {
            var type = typeEnvironment.ResolveType(lastExpression!);
            Console.WriteLine(type);
            Assert.That(type, Is.InstanceOf<T>());
            return this;
        }

        internal TestScenario HasType<T>(Func<T, bool> suchThat) where T : Type
        {
            var type = (T)typeEnvironment.ResolveType(lastExpression!);
            Console.WriteLine(type);
            Assert.That(suchThat(type!), Is.True);

            return this;
        }

        internal TestScenario HasType<T>(string expected) where T : Type
        {
            var type = (T)typeEnvironment.ResolveType(lastExpression!);
            Console.WriteLine(type);
            Assert.That(type, Is.InstanceOf<T>());
            Assert.That(type.ToString(), Is.EqualTo(expected));
            return this;
        }

        internal TestScenario Throws<T>() where T : Exception
        {
            try
            {
                typeEnvironment.ResolveType(lastExpression!);
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