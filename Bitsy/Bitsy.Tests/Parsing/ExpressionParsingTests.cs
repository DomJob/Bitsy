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
        
        Verify<OperationExpression>("(1 & abc(def, 2))");
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
        
        Verify<PrefixExpression>("~func(1)");
    }

    [Test]
    public void DotExpression_Simple()
    {
        ParseExpression("abc.def");
        
        Verify<OperationExpression>("(abc.def)");
    }
    
    [Test]
    public void DotExpression_Chained()
    {
        ParseExpression("abc.def.ghi");
        
        Verify<OperationExpression>("((abc.def).ghi)");
    }
    
    [Test]
    public void DotExpression_Combined()
    {
        ParseExpression("abc.def & a");
        
        Verify<OperationExpression>("((abc.def) & a)");
    }
    
    [Test]
    public void Assignment_Simple()
    {
        ParseExpression("a = 1");
        
        Verify<OperationExpression>("a = 1");
    }
    
    [Test]
    public void Assignment_Complex()
    {
        ParseExpression("a = a & (b | c) ^ abc(def,ghi)");
        
        Verify<OperationExpression>("a = ((a & (b | c)) ^ abc(def, ghi))");
    }

    [Test]
    public void As_Complex()
    {
        ParseExpression("a = a & (b | c) ^ abc(def,ghi) as SomeType");
        
        Verify<OperationExpression>("a = (((a & (b | c)) ^ abc(def, ghi)) as SomeType)");
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
        
        Verify<OperationExpression>("someObject = {a: 1, b: func(arg), c: (abc & d)}");
    }
    
    [Test]
    public void ExplicitObject_Assignment_AndCast()
    {
        ParseExpression("someObject = {a: 1, b: func(arg), c:  abc&d} as Type");
        
        Verify<OperationExpression>("someObject = ({a: 1, b: func(arg), c: (abc & d)} as Type)");
    }
    
    [Test]
    public void ExplicitObject_WrongSyntax()
    {
        try
        {
            ParseExpression("someObject = {a: 1, b, c:  abc&d} as Type");
            Assert.Fail();
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