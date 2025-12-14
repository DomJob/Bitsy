using Bitsy.Lexing;
using Bitsy.Parsing.Expressions;
using Bitsy.Reading;

namespace Bitsy.Parsing;

public class ParserTests
{
    [Test]
    public void SingleIdentifierExpression()
    {
        var code = "abc";
        var expression = ParseExpression(code);
        Assert.That(expression, Is.InstanceOf<IdentifierExpression>());
        Assert.That(expression.ToString(), Is.EqualTo("abc"));
    }

    [Test]
    public void SingleBinaryExpression()
    {
        var code = "a & b";
        var expression = ParseExpression(code);
        Assert.That(expression, Is.InstanceOf<InfixExpression>());
        Assert.That(expression.ToString(), Is.EqualTo("(a & b)"));
    }

    [Test]
    public void UnaryExpression()
    {
        var code = "~a";
        var expression = ParseExpression(code);
        Assert.That(expression, Is.InstanceOf<PrefixExpression>());
        Assert.That(expression.ToString(), Is.EqualTo("~a"));
    }

    [Test]
    public void AssociativeAnd()
    {
        var code = "a & b & c";
        var expression = ParseExpression(code);
        Assert.That(expression, Is.InstanceOf<InfixExpression>());
        Assert.That(expression.ToString(), Is.EqualTo("((a & b) & c)"));
    }

    [Test]
    public void AndOr()
    {
        var code = "a & b | c";
        var expression = ParseExpression(code);
        Assert.That(expression, Is.InstanceOf<InfixExpression>());
        Assert.That(expression.ToString(), Is.EqualTo("((a & b) | c)"));
    }

    [Test]
    public void OrAnd()
    {
        var code = "a | b & c";
        var expression = ParseExpression(code);
        Assert.That(expression, Is.InstanceOf<InfixExpression>());
        Assert.That(expression.ToString(), Is.EqualTo("(a | (b & c))"));
    }

    [Test]
    public void OrAnd_WithForcedPrecedence()
    {
        var code = "(a | b) & c";
        var expression = ParseExpression(code);
        Assert.That(expression, Is.InstanceOf<InfixExpression>());
        Assert.That(expression.ToString(), Is.EqualTo("((a | b) & c)"));
    }

    [Test]
    public void CallExpression_NoParams()
    {
        var code = "abc()";
        var expression = ParseExpression(code);
        Assert.That(expression, Is.InstanceOf<CallExpression>());
        Assert.That(expression.ToString(), Is.EqualTo("abc()"));
    }

    [Test]
    public void CallExpression_OneParam()
    {
        var code = "abc(def)";
        var expression = ParseExpression(code);
        Assert.That(expression, Is.InstanceOf<CallExpression>());
        Assert.That(expression.ToString(), Is.EqualTo("abc(def)"));
    }

    [Test]
    public void CallExpression_TwoParams()
    {
        var code = "abc(def, ghi)";
        var expression = ParseExpression(code);
        Assert.That(expression, Is.InstanceOf<CallExpression>());
        Assert.That(expression.ToString(), Is.EqualTo("abc(def, ghi)"));
    }

    [Test]
    public void CallExpression_ThreeParams_WeirdSpacing()
    {
        var code = "abc(def,       ghi,   jkl)";
        var expression = ParseExpression(code);
        Assert.That(expression, Is.InstanceOf<CallExpression>());
        Assert.That(expression.ToString(), Is.EqualTo("abc(def, ghi, jkl)"));
    }

    [Test]
    public void NegationOnParenthesisExpression()
    {
        var code = "~(a&b)";
        var expression = ParseExpression(code);
        Assert.That(expression, Is.InstanceOf<PrefixExpression>());
        Assert.That(expression.ToString(), Is.EqualTo("~(a & b)"));
    }

    [Test]
    public void CallExpression_OneExpressionParam()
    {
        var code = "abc(def ^ 1)";
        var expression = ParseExpression(code);
        Assert.That(expression, Is.InstanceOf<CallExpression>());
        Assert.That(expression.ToString(), Is.EqualTo("abc((def ^ 1))"));
    }

    [Test]
    public void ObjectExpression_OneElement()
    {
        var code = "[abc]";
        var expression = ParseExpression(code);
        Assert.That(expression, Is.InstanceOf<ObjectExpression>());
        Assert.That(expression.ToString(), Is.EqualTo("[abc]"));
    }

    [Test]
    public void ObjectExpression_TwoElement()
    {
        var code = "[abc,def]";
        var expression = ParseExpression(code);
        Assert.That(expression, Is.InstanceOf<ObjectExpression>());
        Assert.That(expression.ToString(), Is.EqualTo("[abc, def]"));
    }

    [Test]
    public void ObjectExpression_WithExpression()
    {
        var code = "[abc, a&b, def]";
        var expression = ParseExpression(code);
        Assert.That(expression, Is.InstanceOf<ObjectExpression>());
        Assert.That(expression.ToString(), Is.EqualTo("[abc, (a & b), def]"));
    }

    [Test]
    public void ObjectExpression_WithInnerFunctionCall()
    {
        var code = "[abc, a&b, def(1,0,1)]";
        var expression = ParseExpression(code);
        Assert.That(expression, Is.InstanceOf<ObjectExpression>());
        Assert.That(expression.ToString(), Is.EqualTo("[abc, (a & b), def(1, 0, 1)]"));
    }

    [Test]
    public void CallExpression_WithInnerObject()
    {
        var code = "hello([abc, a&b, def(1,0,1)])";
        var expression = ParseExpression(code);
        Assert.That(expression, Is.InstanceOf<CallExpression>());
        Assert.That(expression.ToString(), Is.EqualTo("hello([abc, (a & b), def(1, 0, 1)])"));
    }

    [Test]
    public void NegateObject_ForSomeReason()
    {
        var code = "~[a,b,c]";
        var expression = ParseExpression(code);
        Assert.That(expression, Is.InstanceOf<PrefixExpression>());
        Assert.That(expression.ToString(), Is.EqualTo("~[a, b, c]"));
    }

    [Test]
    public void ConditionalExpression()
    {
        var code = "a ? b : c";
        var expression = ParseExpression(code);
        Assert.That(expression, Is.InstanceOf<ConditionalExpression>());
        Assert.That(expression.ToString(), Is.EqualTo("(a ? b : c)"));
    }

    [Test]
    public void NegateConditionalExpression_Condition()
    {
        var code = "~a ? b : c";
        var expression = ParseExpression(code);
        Assert.That(expression, Is.InstanceOf<ConditionalExpression>());
        Assert.That(expression.ToString(), Is.EqualTo("(~a ? b : c)"));
    }

    [Test]
    public void NegateConditionalExpression_WhenTrue()
    {
        var code = "a ? ~b : c";
        var expression = ParseExpression(code);
        Assert.That(expression, Is.InstanceOf<ConditionalExpression>());
        Assert.That(expression.ToString(), Is.EqualTo("(a ? ~b : c)"));
    }

    [Test]
    public void NegateConditionalExpression_WhenFalse()
    {
        var code = "a ? b : ~c";
        var expression = ParseExpression(code);
        Assert.That(expression, Is.InstanceOf<ConditionalExpression>());
        Assert.That(expression.ToString(), Is.EqualTo("(a ? b : ~c)"));
    }

    [Test]
    public void ConditionalExpression_AsArgument()
    {
        var code = "func(a ? b : c)";
        var expression = ParseExpression(code);
        Assert.That(expression, Is.InstanceOf<CallExpression>());
        Assert.That(expression.ToString(), Is.EqualTo("func((a ? b : c))"));
    }

    [Test]
    public void ConditionalExpression_AsTwoArguments()
    {
        var code = "func(a ? b : c, d?e:f)";
        var expression = ParseExpression(code);
        Assert.That(expression, Is.InstanceOf<CallExpression>());
        Assert.That(expression.ToString(), Is.EqualTo("func((a ? b : c), (d ? e : f))"));
    }

    [Test]
    public void ConditionalExpression_AsObject()
    {
        var code = "[a ? b : c, d?e:f]";
        var expression = ParseExpression(code);
        Assert.That(expression, Is.InstanceOf<ObjectExpression>());
        Assert.That(expression.ToString(), Is.EqualTo("[(a ? b : c), (d ? e : f)]"));
    }

    [Test]
    public void ChainedConditionalExpression()
    {
        var code = "a ? b : c ? d : e";
        var expression = ParseExpression(code);
        Assert.That(expression, Is.InstanceOf<ConditionalExpression>());
        Assert.That(expression.ToString(), Is.EqualTo("(a ? b : (c ? d : e))"));
    }

    [Test]
    public void ChainedConditionalExpression_Inversed()
    {
        var code = "(a ? b : c) ? d : e";
        var expression = ParseExpression(code);
        Assert.That(expression, Is.InstanceOf<ConditionalExpression>());
        Assert.That(expression.ToString(), Is.EqualTo("((a ? b : c) ? d : e)"));
    }

    [Test]
    public void AsExpression_Simple()
    {
        var code = "abc as SomeType";
        var expression = ParseExpression(code);
        Assert.That(expression, Is.InstanceOf<AsExpression>());
        Assert.That(expression.ToString(), Is.EqualTo("(abc as SomeType)"));
    }

    [Test]
    public void AsExpression_InInfix()
    {
        var code = "abc as Bit ^ other";
        var expression = ParseExpression(code);
        //Assert.That(expression, Is.InstanceOf<InfixExpression>());
        Assert.That(expression.ToString(), Is.EqualTo("((abc as Bit) ^ other)"));
    }

    [Test]
    public void AsExpression_InInfix2()
    {
        var code = "other ^ abc as Bit";
        var expression = ParseExpression(code);
        Assert.That(expression.ToString(), Is.EqualTo("((other ^ abc) as Bit)"));
        Assert.That(expression, Is.InstanceOf<AsExpression>());
    }

    [Test]
    public void AsExpression_Chained()
    {
        var code = "abc as SomeType as OtherType";
        var expression = ParseExpression(code);
        Assert.That(expression, Is.InstanceOf<AsExpression>());
        Assert.That(expression.ToString(), Is.EqualTo("((abc as SomeType) as OtherType)"));
    }

    [Test]
    public void AsExpression_ProperOrder()
    {
        var code = "a | b & c as Type";
        var expression = ParseExpression(code);
        Assert.That(expression.ToString(), Is.EqualTo("((a | (b & c)) as Type)"));
        Assert.That(expression, Is.InstanceOf<AsExpression>());
    }

    [Test]
    public void AsExpression_OfAConditional()
    {
        var code = "a ? b : c as Type";
        var expression = ParseExpression(code);
        Assert.That(expression, Is.InstanceOf<AsExpression>());
        Assert.That(expression.ToString(), Is.EqualTo("((a ? b : c) as Type)"));
    }
    
    [Test]
    public void DotExpression_Simple()
    {
        var code = "element.attribute";
        var expression = ParseExpression(code);
        Assert.That(expression, Is.InstanceOf<DotExpression>());
        Assert.That(expression.ToString(), Is.EqualTo("(element.attribute)"));
    }

    [Test]
    public void DotExpression_InInfixExpr()
    {
        var code = "a & element.attribute";
        var expression = ParseExpression(code);
        Assert.That(expression, Is.InstanceOf<InfixExpression>());
        Assert.That(expression.ToString(), Is.EqualTo("(a & (element.attribute))"));
    }

    [Test]
    public void DotExpression_Two_InInfixExpr()
    {
        var code = "integer.b1 & integer.b2";
        var expression = ParseExpression(code);
        Assert.That(expression, Is.InstanceOf<InfixExpression>());
        Assert.That(expression.ToString(), Is.EqualTo("((integer.b1) & (integer.b2))"));
    }

    [Test]
    public void DotExpression_InConditional()
    {
        var code = "integer.b1 ? a.b : c.d";
        var expression = ParseExpression(code);
        Assert.That(expression, Is.InstanceOf<ConditionalExpression>());
        Assert.That(expression.ToString(), Is.EqualTo("((integer.b1) ? (a.b) : (c.d))"));
    }

    [Test]
    public void DotExpression_chained()
    {
        var code = "anObject.attribute.next";
        var expression = ParseExpression(code);
        Assert.That(expression, Is.InstanceOf<DotExpression>());
        Assert.That(((DotExpression)expression).Attribute.Identifier.Literal, Is.EqualTo("next"));
        Assert.That(expression.ToString(), Is.EqualTo("((anObject.attribute).next)"));
    }

    [Test]
    public void DotExpression_Negated()
    {
        var code = "~element.attribute";
        var expression = ParseExpression(code);
        Assert.That(expression, Is.InstanceOf<PrefixExpression>());
        Assert.That(expression.ToString(), Is.EqualTo("~(element.attribute)"));
    }

    [Test]
    public void DotExpression_TargetIsFunctionCall()
    {
        var code = "(someFunc(a,b)).attribute";
        var expression = ParseExpression(code);
        Assert.That(expression, Is.InstanceOf<DotExpression>());
        Assert.That(expression.ToString(), Is.EqualTo("(someFunc(a, b).attribute)"));
    }

    [Test]
    public void UnhappyPath_MissingParenthesis()
    {
        var code = "(a ? b : c ? d : e";
        try
        {
            var expression = ParseExpression(code);
            Assert.Fail();
        }
        catch (ParserException e)
        {
            Assert.Pass();
        }
    }

    [Test]
    public void UnhappyPath_MissingBracket()
    {
        var code = "function([1,2,3) & 2";
        try
        {
            var expression = ParseExpression(code);
            Assert.Fail();
        }
        catch (ParserException e)
        {
            Assert.Pass();
        }
    }

    [Test]
    public void UnhappyPath_BrokenConditional()
    {
        var code = "a ? b & c";
        try
        {
            var expression = ParseExpression(code);
            Assert.Fail();
        }
        catch (ParserException e)
        {
            Assert.Pass();
        }
    }

    private Expression ParseExpression(string code)
    {
        var reader = new StringCodeReader(code);
        var lexer = new Lexer(reader);
        return new Parser(lexer).ParseExpression();
    }
}