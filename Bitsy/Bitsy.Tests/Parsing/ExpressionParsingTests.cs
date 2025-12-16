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

        Verify<UnaryExpression>("~abc");
    }

    [Test]
    public void Infix_AndOperation()
    {
        ParseExpression("a & b");

        Verify<BinaryExpression>("(a & b)");
    }

    [Test]
    public void Infix_OrOperation()
    {
        ParseExpression("a | b");

        Verify<BinaryExpression>("(a | b)");
    }

    [Test]
    public void Infix_XorOperation()
    {
        ParseExpression("a ^ b");

        Verify<BinaryExpression>("(a ^ b)");
    }

    [Test]
    public void NegateWithAndOperation()
    {
        ParseExpression("~a & b");

        Verify<BinaryExpression>("(~a & b)");
    }
    
    [Test]
    public void ReturnStatement()
    {
        ParseExpression("return a");

        Verify<UnaryExpression>("return a");
    }
    
    [Test]
    public void ReturnStatement_Precedence()
    {
        ParseExpression("return a&b");

        Verify<UnaryExpression>("return (a & b)");
    }
    
    [Test]
    public void ReturnStatement_Precedence2()
    {
        ParseExpression("return ~a");

        Verify<UnaryExpression>("return ~a");
    }
    
    [Test]
    public void ReturnStatement_Precedence3()
    {
        ParseExpression("return ~a as SomeType");

        Verify<UnaryExpression>("return (~a as SomeType)");
    }

    [Test]
    public void ReturnStatement_Precedence4()
    {
        ParseExpression("return a=1");

        Verify<UnaryExpression>("return a = 1");
    }
    
    [Test]
    public void NegateTwice()
    {
        ParseExpression("~~a");

        Verify<UnaryExpression>("~~a");
    }

    [Test]
    public void NegateAndOperation()
    {
        ParseExpression("~(a & b)");

        Verify<UnaryExpression>("~(a & b)");
    }

    [Test]
    public void AndOperationsLeftAssociative()
    {
        ParseExpression("a & b & c");

        Verify<BinaryExpression>("((a & b) & c)");
    }

    [Test]
    public void AndOrOperationsProperOrder()
    {
        ParseExpression("a | b & c");

        Verify<BinaryExpression>("(a | (b & c))");
    }

    [Test]
    public void AndXOrOperationsProperOrder()
    {
        ParseExpression("a ^ b & c");

        Verify<BinaryExpression>("(a ^ (b & c))");
    }

    [Test]
    public void CallExpression_NoArgs()
    {
        ParseExpression("abc()");

        Verify<CallExpression>("abc()");
    }

    [Test]
    public void CallExpression_OneArgs()
    {
        ParseExpression("abc(1)");

        Verify<CallExpression>("abc(1)");
    }

    [Test]
    public void CallExpression_TwoArgs()
    {
        ParseExpression("abc(def, 2)");

        Verify<CallExpression>("abc(def, 2)");
    }

    [Test]
    public void CallExpression_ThreeArgs_Nested()
    {
        ParseExpression("abc(def, ghi(3))");

        Verify<CallExpression>("abc(def, ghi(3))");
    }

    [Test]
    public void CombineCallAnd()
    {
        ParseExpression("1 & abc(def, 2)");

        Verify<BinaryExpression>("(1 & abc(def, 2))");
    }

    [Test]
    public void ComplexCall()
    {
        ParseExpression("(def(a^b)&1)(abc)");

        Verify<CallExpression>("(def((a ^ b)) & 1)(abc)");
    }

    [Test]
    public void NegateCall()
    {
        ParseExpression("~func(1)");

        Verify<UnaryExpression>("~func(1)");
    }

    [Test]
    public void DotExpression_Simple()
    {
        ParseExpression("abc.def");

        Verify<BinaryExpression>("(abc.def)");
    }

    [Test]
    public void DotExpression_Chained()
    {
        ParseExpression("abc.def.ghi");

        Verify<BinaryExpression>("((abc.def).ghi)");
    }

    [Test]
    public void DotExpression_Combined()
    {
        ParseExpression("abc.def & a");

        Verify<BinaryExpression>("((abc.def) & a)");
    }

    [Test]
    public void Assignment_Simple()
    {
        ParseExpression("a = 1");

        Verify<BinaryExpression>("a = 1");
    }

    [Test]
    public void Assignment_Complex()
    {
        ParseExpression("a = a & (b | c) ^ abc(def,ghi)");

        Verify<BinaryExpression>("a = ((a & (b | c)) ^ abc(def, ghi))");
    }

    [Test]
    public void As_Complex()
    {
        ParseExpression("a = a & (b | c) ^ abc(def,ghi) as SomeType");

        Verify<BinaryExpression>("a = (((a & (b | c)) ^ abc(def, ghi)) as SomeType)");
    }

    [Test]
    public void ExplicitObject_Simple()
    {
        ParseExpression("{a: 1, b:2, c:  3}");

        Verify<ExplicitObjectExpression>("{a: 1, b: 2, c: 3}");
    }

    [Test]
    public void ExplicitObject_LessSimple()
    {
        ParseExpression("{a: 1, b: func(arg), c:  abc&d}");

        Verify<ExplicitObjectExpression>("{a: 1, b: func(arg), c: (abc & d)}");
    }

    [Test]
    public void ExplicitObject_Assignment()
    {
        ParseExpression("someObject = {a: 1, b: func(arg), c:  abc&d}");

        Verify<BinaryExpression>("someObject = {a: 1, b: func(arg), c: (abc & d)}");
    }

    [Test]
    public void ExplicitObject_Assignment_AndCast()
    {
        ParseExpression("someObject = {a: 1, b: func(arg), c:  abc&d} as Type");

        Verify<BinaryExpression>("someObject = ({a: 1, b: func(arg), c: (abc & d)} as Type)");
    }

    [Test]
    public void ExplicitObject_WrongSyntax()
    {
        try
        {
            ParseExpression("someObject = {a: 1, b, c:  abc&d} as Type");
            Assert.That(expression, Is.Null);
        }
        catch (ParserException e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.Token);
            Assert.Pass();
        }
    }

    [Test]
    public void ImplicitObject_Simple()
    {
        ParseExpression("{0,1,0,1}");

        Verify<ImplicitObjectExpression>("{0, 1, 0, 1}");
    }

    [Test]
    public void ImplicitObject_Complex()
    {
        ParseExpression("{0,{0,1} as Bit2,abc(a),1^0, {b1:0,b2:1,b3:0,b4: 2}}");

        Verify<ImplicitObjectExpression>("{0, ({0, 1} as Bit2), abc(a), (1 ^ 0), {b1: 0, b2: 1, b3: 0, b4: 2}}");
    }

    [Test]
    public void Conditional_Simple()
    {
        ParseExpression("a ? b : c");

        Verify<ConditionalExpression>("(a ? b : c)");
    }

    [Test]
    public void Conditional_Chained()
    {
        ParseExpression("cond1 ? doThis : otherwise ? doThat : giveUp");

        Verify<ConditionalExpression>("(cond1 ? doThis : (otherwise ? doThat : giveUp))");
    }

    [Test]
    public void Conditional_ChainedTheOtherWay()
    {
        ParseExpression("cond1 ? andCond2 ? beAmazed : beLessAmazed : scream");

        Verify<ConditionalExpression>("(cond1 ? (andCond2 ? beAmazed : beLessAmazed) : scream)");
    }

    [Test]
    public void Conditional_WithOtherStuff()
    {
        ParseExpression("a & cond ? 0 : 1 ^ b");

        Verify<ConditionalExpression>("((a & cond) ? 0 : (1 ^ b))");
    }

    [Test]
    public void Conditional_WithOtherStuffForced()
    {
        ParseExpression("a & (cond ? 0 : 1) ^ b");

        Verify<BinaryExpression>("((a & (cond ? 0 : 1)) ^ b)");
    }

    [Test]
    public void FunctionDeclaration_Simplest()
    {
        ParseExpression("someFunc() { a = 1 }");
        
        Verify<FunctionDeclaration>("someFunc() { a = 1 }");
    }
    
    [Test]
    public void FunctionDeclaration_WithArg()
    {
        ParseExpression("someFunc(Bit a) { b = a & 1 }");
        
        Verify<FunctionDeclaration>("someFunc(Bit a) { b = (a & 1) }");
    }
    
    [Test]
    public void FunctionDeclaration_WithFunctionalArg()
    {
        ParseExpression("someFunc(Bit->Bit a) { b = a() }");
        
        Verify<FunctionDeclaration>("someFunc(Bit->Bit a) { b = a() }");
    }
    
    [Test]
    public void FunctionDeclaration_WithTwoFunctionalArgs()
    {
        ParseExpression("someFunc(Bit->Bit a, ()->Bit b) { c = a(1) ^ b()  }");
        
        Verify<FunctionDeclaration>("someFunc(Bit->Bit a, ()->Bit b) { c = (a(1) ^ b()) }");
    }
    
    [Test]
    public void FunctionDeclaration_WithArgs()
    {
        ParseExpression("someFunc(Bit a, SomeType b) { c = a & b.idk d=1 }");
        
        Verify<FunctionDeclaration>("someFunc(Bit a, SomeType b) { c = (a & (b.idk)) d = 1 }");
    }

    [Test]
    public void TypeExpression_Simple()
    {
        ParseExpression("a->b");
        
        Verify<TypeName>("a->b");
    }

    [Test]
    public void TypeExpression_Empty()
    {
        ParseExpression("()->a");
        
        Verify<TypeName>("()->a");
    }
    
    [Test]
    public void TypeExpression_TwoInputs()
    {
        ParseExpression("(a,b)->c");
        
        Verify<TypeName>("(a, b)->c");
    }

    [Test]
    public void TypeDeclaration_Simple()
    {
        ParseExpression("SomeType { Bit b }");
        Verify<TypeDeclaration>("SomeType { Bit b }");
    }
    
    [Test]
    public void TypeDeclaration_Multiple()
    {
        ParseExpression("SomeType { Bit b OtherType c }");
        Verify<TypeDeclaration>("SomeType { Bit b OtherType c }");
    }
    
    [Test]
    public void TypeDeclaration_Template_Simple()
    {
        ParseExpression("SomeType<T> { T b }");
        Verify<TypeDeclaration>("SomeType<T> { T b }");
    }
    
    [Test]
    public void TypeDeclaration_Template_Multiple()
    {
        ParseExpression("SomeType<T> { T b OtherType c }");
        Verify<TypeDeclaration>("SomeType<T> { T b OtherType c }");
    }
    
    [Test]
    public void TypeDeclaration_MultiTemplate_Multiple()
    {
        ParseExpression("SomeType<T,V> { T b V c }");
        Verify<TypeDeclaration>("SomeType<T, V> { T b V c }");
    }
    
    [Test]
    public void NameExpression_Templated()
    {
        ParseExpression("SomeType<T,V>");
        Verify<NameExpression>("SomeType<T, V>");
    }
    
    [Test]
    public void TypeDeclaration_WithTemplatedBody()
    {
        ParseExpression("SomeType { List<Bit> bits }");
        Verify<TypeDeclaration>("SomeType { List<Bit> bits }");
    }
    
    [Test]
    public void TypeDeclaration_WithCallableBody()
    {
        ParseExpression("SomeType<T> { T->List<Bit> bits }");
        Verify<TypeDeclaration>("SomeType<T> { T->List<Bit> bits }");
    }
    
    private void Verify<T>(string value) where T : Expression
    {
        Console.WriteLine(expression);
        Assert.That(expression.ToString(), Is.EqualTo(value));
        Assert.That(expression, Is.InstanceOf<T>());
    }

    private void Verify(string value)
    {
        Verify<Expression>(value);
    }

    private Expression ParseExpression(string code)
    {
        var reader = new StringCodeReader(code);
        var lexer = new Lexer(reader);
        var parser = new Parser(lexer);
        expression = parser.ParseExpression();
        return expression;
    }
}