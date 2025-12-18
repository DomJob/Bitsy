using Bitsy.Lexing;
using Bitsy.Parsing;
using Bitsy.Parsing.Expressions;
using Bitsy.Reading;

namespace Bitsy;

public class ParserTests
{
    private Expression expression;

    [Test]
    public void Identifier_Simple()
    {
        ParseStatement("abc");

        Verify<NameExpression>("abc");
    }

    [Test]
    public void Prefix_NotExpression()
    {
        ParseStatement("~abc");

        Verify<UnaryExpression>("~abc");
    }

    [Test]
    public void Infix_AndOperation()
    {
        ParseStatement("a & b");

        Verify<BinaryExpression>("(a & b)");
    }

    [Test]
    public void Infix_OrOperation()
    {
        ParseStatement("a | b");

        Verify<BinaryExpression>("(a | b)");
    }

    [Test]
    public void Infix_XorOperation()
    {
        ParseStatement("a ^ b");

        Verify<BinaryExpression>("(a ^ b)");
    }

    [Test]
    public void NegateWithAndOperation()
    {
        ParseStatement("~a & b");

        Verify<BinaryExpression>("(~a & b)");
    }

    [Test]
    public void ReturnStatement()
    {
        ParseStatement("return a");

        Verify<UnaryExpression>("return a");
    }

    [Test]
    public void ReturnStatement_Precedence()
    {
        ParseStatement("return a&b");

        Verify<UnaryExpression>("return (a & b)");
    }

    [Test]
    public void ReturnStatement_Precedence2()
    {
        ParseStatement("return ~a");

        Verify<UnaryExpression>("return ~a");
    }

    [Test]
    public void ReturnStatement_Precedence3()
    {
        ParseStatement("return ~a as SomeType");

        Verify<UnaryExpression>("return (~a as SomeType)");
    }

    [Test]
    public void NegateTwice()
    {
        ParseStatement("~~a");

        Verify<UnaryExpression>("~~a");
    }

    [Test]
    public void NegateAndOperation()
    {
        ParseStatement("~(a & b)");

        Verify<UnaryExpression>("~(a & b)");
    }

    [Test]
    public void AndOperationsLeftAssociative()
    {
        ParseStatement("a & b & c");

        Verify<BinaryExpression>("((a & b) & c)");
    }

    [Test]
    public void AndOrOperationsProperOrder()
    {
        ParseStatement("a | b & c");

        Verify<BinaryExpression>("(a | (b & c))");
    }

    [Test]
    public void AndXOrOperationsProperOrder()
    {
        ParseStatement("a ^ b & c");

        Verify<BinaryExpression>("(a ^ (b & c))");
    }

    [Test]
    public void CallExpression_NoArgs()
    {
        ParseStatement("abc()");

        Verify<CallExpression>("abc()");
    }

    [Test]
    public void CallExpression_OneArgs()
    {
        ParseStatement("abc(1)");

        Verify<CallExpression>("abc(1)");
    }

    [Test]
    public void CallExpression_TwoArgs()
    {
        ParseStatement("abc(def, 2)");

        Verify<CallExpression>("abc(def, 2)");
    }

    [Test]
    public void CallExpression_ThreeArgs_Nested()
    {
        ParseStatement("abc(def, ghi(3))");

        Verify<CallExpression>("abc(def, ghi(3))");
    }

    [Test]
    public void CombineCallAnd()
    {
        ParseStatement("1 & abc(def, 2)");

        Verify<BinaryExpression>("(1 & abc(def, 2))");
    }

    [Test]
    public void ComplexCall()
    {
        ParseStatement("(def(a^b)&1)(abc)");

        Verify<CallExpression>("(def((a ^ b)) & 1)(abc)");
    }

    [Test]
    public void NegateCall()
    {
        ParseStatement("~func(1)");

        Verify<UnaryExpression>("~func(1)");
    }

    [Test]
    public void DotExpression_Simple()
    {
        ParseStatement("abc.def");

        Verify<BinaryExpression>("(abc.def)");
    }

    [Test]
    public void DotExpression_Chained()
    {
        ParseStatement("abc.def.ghi");

        Verify<BinaryExpression>("((abc.def).ghi)");
    }

    [Test]
    public void DotExpression_Combined()
    {
        ParseStatement("abc.def & a");

        Verify<BinaryExpression>("((abc.def) & a)");
    }

    [Test]
    public void Assignment_Simple()
    {
        ParseStatement("a = 1");

        Verify<BinaryExpression>("a = 1");
    }

    [Test]
    public void Assignment_Complex()
    {
        ParseStatement("a = a & (b | c) ^ abc(def,ghi)");

        Verify<BinaryExpression>("a = ((a & (b | c)) ^ abc(def, ghi))");
    }

    [Test]
    public void As_Complex()
    {
        ParseStatement("a & (b | c) ^ abc(def,ghi) as SomeType");

        Verify<BinaryExpression>("(((a & (b | c)) ^ abc(def, ghi)) as SomeType)");
    }

    [Test]
    public void ExplicitObject_Simple()
    {
        ParseStatement("{a: 1, b:2, c:  3}");

        Verify<ExplicitObjectExpression>("{a: 1, b: 2, c: 3}");
    }

    [Test]
    public void ExplicitObject_LessSimple()
    {
        ParseStatement("{a: 1, b: func(arg), c:  abc&d}");

        Verify<ExplicitObjectExpression>("{a: 1, b: func(arg), c: (abc & d)}");
    }

    [Test]
    public void ExplicitObject_Assignment()
    {
        ParseStatement("someObject = {a: 1, b: func(arg), c:  abc&d}");

        Verify<BinaryExpression>("someObject = {a: 1, b: func(arg), c: (abc & d)}");
    }

    [Test]
    public void ExplicitObject_Assignment_AndCast()
    {
        ParseStatement("someObject = {a: 1, b: func(arg), c:  abc&d} as Type");

        Verify<BinaryExpression>("someObject = ({a: 1, b: func(arg), c: (abc & d)} as Type)");
    }

    [Test]
    public void ExplicitObject_WrongSyntax()
    {
        VerifyThrows<ParserException>("{a: 1, b, c:  abc&d} as Type");
    }

    [Test]
    public void ImplicitObject_Simple()
    {
        ParseStatement("{0,1,0,1}");

        Verify<ImplicitObjectExpression>("{0, 1, 0, 1}");
    }

    [Test]
    public void ImplicitObject_Complex()
    {
        ParseStatement("{0,{0,1} as Bit2,abc(a),1^0, {b1:0,b2:1,b3:0,b4: 2}}");

        Verify<ImplicitObjectExpression>("{0, ({0, 1} as Bit2), abc(a), (1 ^ 0), {b1: 0, b2: 1, b3: 0, b4: 2}}");
    }

    [Test]
    public void Conditional_Simple()
    {
        ParseStatement("a ? b : c");

        Verify<ConditionalExpression>("(a ? b : c)");
    }

    [Test]
    public void Conditional_Chained()
    {
        ParseStatement("cond1 ? doThis : otherwise ? doThat : giveUp");

        Verify<ConditionalExpression>("(cond1 ? doThis : (otherwise ? doThat : giveUp))");
    }

    [Test]
    public void Conditional_ChainedTheOtherWay()
    {
        ParseStatement("cond1 ? andCond2 ? beAmazed : beLessAmazed : scream");

        Verify<ConditionalExpression>("(cond1 ? (andCond2 ? beAmazed : beLessAmazed) : scream)");
    }

    [Test]
    public void Conditional_WithOtherStuff()
    {
        ParseStatement("a & cond ? 0 : 1 ^ b");

        Verify<ConditionalExpression>("((a & cond) ? 0 : (1 ^ b))");
    }

    [Test]
    public void Conditional_WithOtherStuffForced()
    {
        ParseStatement("a & (cond ? 0 : 1) ^ b");

        Verify<BinaryExpression>("((a & (cond ? 0 : 1)) ^ b)");
    }

    [Test]
    public void FunctionDeclaration_Simplest()
    {
        ParseStatement("someFunc() { a = 1 }");

        Verify<FunctionDeclaration>("someFunc() { a = 1 }");
    }

    [Test]
    public void FunctionDeclaration_MissingBody_Throws()
    {
        VerifyThrows<ParserException>("someFunc(Bit a, Bit b)");
    }

    [Test]
    public void FunctionDeclaration_WithArg()
    {
        ParseStatement("someFunc(Bit a) { b = a & 1 }");

        Verify<FunctionDeclaration>("someFunc(Bit a) { b = (a & 1) }");
    }

    [Test]
    public void FunctionDeclaration_WithFunctionalArg()
    {
        ParseStatement("someFunc(Bit->Bit a) { b = a() }");

        Verify<FunctionDeclaration>("someFunc((Bit->Bit) a) { b = a() }");
    }

    [Test]
    public void FunctionDeclaration_WithNestedInputFunctionalArg()
    {
        ParseStatement("someFunc((Bit->Bit)->Bit a) { b = a() }");

        Verify<FunctionDeclaration>("someFunc(((Bit->Bit)->Bit) a) { b = a() }");
    }

    [Test]
    public void FunctionDeclaration_WithNestedOutputFunctionalArg()
    {
        ParseStatement("someFunc(Bit->(Bit->Bit) a) { b = a() }");

        Verify<FunctionDeclaration>("someFunc((Bit->(Bit->Bit)) a) { b = a() }");
    }

    [Test]
    public void FunctionDeclaration_WithTwoFunctionalArgs()
    {
        ParseStatement("someFunc(Bit->Bit a, ()->Bit b) { c = a(1) ^ b()  }");

        Verify<FunctionDeclaration>("someFunc((Bit->Bit) a, (()->Bit) b) { c = (a(1) ^ b()) }");
    }

    [Test]
    public void FunctionDeclaration_WithArgs()
    {
        ParseStatement("someFunc(Bit a, SomeType b) { c = a & b.idk d=1 }");

        Verify<FunctionDeclaration>("someFunc(Bit a, SomeType b) { c = (a & (b.idk)) d = 1 }");
    }

    [Test]
    public void TypeExpression_SimpleUnion()
    {
        ParseStatement("(a,b)");

        Verify<TypeExpression>("(a, b)");
    }

    [Test]
    public void TypeExpression_SimpleFunctionalType()
    {
        ParseStatement("a->b");

        Verify<FunctionTypeExpression>("(a->b)");
    }

    [Test]
    public void TypeExpression_EmptyInput()
    {
        ParseStatement("()->a");

        Verify<FunctionTypeExpression>("(()->a)");
    }

    [Test]
    public void TypeExpression_EmptyInputTemplatedOutput()
    {
        ParseStatement("()->SomeType<T>");

        Verify<FunctionTypeExpression>("(()->SomeType<T>)");
    }

    [Test]
    public void TypeExpression_EmptyOutput()
    {
        ParseStatement("a->()");

        Verify<FunctionTypeExpression>("(a->())");
    }

    [Test]
    public void TypeExpression_TwoInputs()
    {
        ParseStatement("(a,b)->c");

        Verify<FunctionTypeExpression>("((a, b)->c)");
    }

    [Test]
    public void TypeExpression_Associativity()
    {
        ParseStatement("a->b->c");

        Verify<FunctionTypeExpression>("(a->(b->c))");
    }

    [Test]
    public void TypeExpression_NestedInputs()
    {
        ParseStatement("(a,(b->c))->d");

        Verify<FunctionTypeExpression>("((a, (b->c))->d)");
    }

    [Test]
    public void TypeExpression_Templated()
    {
        ParseStatement("SomeType<T>");
        Verify<TypeExpression>("SomeType<T>");
    }

    [Test]
    public void TypeExpression_TemplatedInFunc()
    {
        ParseStatement("SomeType<T>->Output");
        Verify<TypeExpression>("(SomeType<T>->Output)");
    }

    [Test]
    public void TypeExpression_TemplatedTwice()
    {
        ParseStatement("SomeType<T,V>");
        Verify<TypeExpression>("SomeType<T, V>");
    }

    [Test]
    public void TypeDeclaration_Simple()
    {
        ParseStatement("SomeType { Bit b }");
        Verify<TypeDeclaration>("SomeType { Bit b }");
    }

    [Test]
    public void TypeDeclaration_Multiple()
    {
        ParseStatement("SomeType { Bit b OtherType c }");
        Verify<TypeDeclaration>("SomeType { Bit b OtherType c }");
    }

    [Test]
    public void TypeDeclaration_Template_Simple()
    {
        ParseStatement("SomeType<T> { T b }");
        Verify<TypeDeclaration>("SomeType<T> { T b }");
    }

    [Test]
    public void TypeDeclaration_Template_UnionQuestionMark()
    {
        ParseStatement("SomeType<(T,V)> { T b }");
        Verify<TypeDeclaration>("SomeType<(T, V)> { T b }");
    }

    [Test]
    public void TypeDeclaration_RecursiveTemplate()
    {
        ParseStatement("List<T> { T current List<T> rest }");
        Verify<TypeDeclaration>("List<T> { T current List<T> rest }");
    }

    [Test]
    public void TypeDeclaration_Template_Multiple()
    {
        ParseStatement("SomeType<T> { T b OtherType c }");
        Verify<TypeDeclaration>("SomeType<T> { T b OtherType c }");
    }

    [Test]
    public void TypeDeclaration_MultiTemplate_Multiple()
    {
        ParseStatement("SomeType<T,V> { T b V c }");
        Verify<TypeDeclaration>("SomeType<T, V> { T b V c }");
    }

    [Test]
    public void TypeDeclaration_WithTemplatedBody()
    {
        ParseStatement("SomeType { List<Bit> bits }");
        Verify<TypeDeclaration>("SomeType { List<Bit> bits }");
    }

    [Test]
    public void TypeDeclaration_WithCallableBody()
    {
        ParseStatement("SomeType<T> { T->List<Bit> bits }");
        Verify<TypeDeclaration>("SomeType<T> { (T->List<Bit>) bits }");
    }

    [Test]
    public void SyntaxError_NotNameOnDotExpression()
    {
        VerifyThrows<SyntaxError>("abc.(efg & 1)");
    }

    private T VerifyThrows<T>(string code) where T : Exception
    {
        try
        {
            ParseStatement(code);
            Assert.Fail();
            return null;
        }
        catch (T e)
        {
            return e;
        }
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

    private Expression ParseStatement(string code)
    {
        var reader = new LineReader(code);
        var lexer = new Lexer(reader);
        var parser = new Parser(lexer);
        expression = parser.ParseStatement();
        return expression;
    }
}