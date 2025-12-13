using Bitsy.Reading;

namespace Bitsy.Lexing;

public class LexerTests
{
    [Test]
    public void EmptyString_TokenEOF() =>
        WhenCodeIs("")
        .Then
        .NextToken
        .IsOfType(TokenType.End);
    
    [Test]
    public void StringLiteral_Identifier() =>
        WhenCodeIs("helloWorld")
            .Then
            .NextToken
            .IsOfType(TokenType.Identifier)
            .And.HasValue("helloWorld")
            .And.NextToken.IsOfType(TokenType.End);

    [Test]
    public void AssignmentTest() =>
        WhenCodeIs("someConstant = previousConstant    & 1")
            .Then.NextToken.IsOfType(TokenType.Identifier).And.HasValue("someConstant")
            .And.NextToken.IsOfType(TokenType.Assignment)
            .And.NextToken.IsOfType(TokenType.Identifier).And.HasValue("previousConstant")
            .And.NextToken.IsOfType(TokenType.And)
            .And.NextToken.IsOfType(TokenType.Identifier).And.HasValue("1");

    [Test]
    public void IgnoreCommentTest() =>
        WhenCodeIs("someConstant =/*comment comment ignored*/ previousConstant    & 1")
            .Then.NextToken.IsOfType(TokenType.Identifier).And.HasValue("someConstant")
            .And.NextToken.IsOfType(TokenType.Assignment)
            .And.NextToken.IsOfType(TokenType.Identifier).And.HasValue("previousConstant")
            .And.NextToken.IsOfType(TokenType.And)
            .And.NextToken.IsOfType(TokenType.Identifier).And.HasValue("1");

    [Test]
    public void TernaryOperatorTokens() =>
        WhenCodeIs("someConstant = someBit ? abc : def()")
            .Then.NextToken.IsOfType(TokenType.Identifier).And.HasValue("someConstant")
            .And.NextToken.IsOfType(TokenType.Assignment)
            .And.NextToken.IsOfType(TokenType.Identifier).And.HasValue("someBit")
            .And.NextToken.IsOfType(TokenType.Question)
            .And.NextToken.IsOfType(TokenType.Identifier).And.HasValue("abc")
            .And.NextToken.IsOfType(TokenType.Colon)
            .And.NextToken.IsOfType(TokenType.Identifier).And.HasValue("def")
            .And.NextToken.IsOfType(TokenType.LeftParenthesis)
            .And.NextToken.IsOfType(TokenType.RightParenthesis)
            .And.NextToken.IsOfType(TokenType.End);

    [Test]
    public void MiscMashup() =>
        WhenCodeIs("a.b &^~|<> {}][")
            .Then.NextToken.IsOfType(TokenType.Identifier).And.HasValue("a")
            .And.NextToken.IsOfType(TokenType.Dot)
            .And.NextToken.IsOfType(TokenType.Identifier).And.HasValue("b")
            .And.NextToken.IsOfType(TokenType.And)
            .And.NextToken.IsOfType(TokenType.Xor)
            .And.NextToken.IsOfType(TokenType.Not)
            .And.NextToken.IsOfType(TokenType.Or)
            .And.NextToken.IsOfType(TokenType.LeftAngle)
            .And.NextToken.IsOfType(TokenType.RightAngle)
            .And.NextToken.IsOfType(TokenType.LeftBrace)
            .And.NextToken.IsOfType(TokenType.RightBrace)
            .And.NextToken.IsOfType(TokenType.RightBracket)
            .And.NextToken.IsOfType(TokenType.LeftBracket)
            .And.NextToken.IsOfType(TokenType.End);

    [Test]
    public void SingleLineComment_SkipRestOfLine() =>
        WhenCodeIs("a = b // this whole line is ignored\ndef")
            .Then.NextToken.IsOfType(TokenType.Identifier).And.HasValue("a")
            .And.NextToken.IsOfType(TokenType.Assignment)
            .And.NextToken.IsOfType(TokenType.Identifier).And.HasValue("b")
            .And.NextToken.IsOfType(TokenType.Identifier).And.HasValue("def");

    [Test]
    public void SingleLineComment_NoMoreLines_SendEof() =>
        WhenCodeIs("a = b // this whole line is ignored")
            .Then.NextToken.IsOfType(TokenType.Identifier).And.HasValue("a")
            .And.NextToken.IsOfType(TokenType.Assignment)
            .And.NextToken.IsOfType(TokenType.Identifier).And.HasValue("b")
            .And.NextToken.IsOfType(TokenType.End);

    [Test]
    public void LiteralKeywords() =>
        WhenCodeIs("return abc as Bit")
            .Then.NextToken.IsOfType(TokenType.Return)
            .And.NextToken.IsOfType(TokenType.Identifier).And.HasValue("abc")
            .And.NextToken.IsOfType(TokenType.As)
            .And.NextToken.IsOfType(TokenType.Identifier).And.HasValue("Bit");
    
    private static LexerTestScenario WhenCodeIs(String code)
    {
        var reader = new StringCodeReader(code);
        return new LexerTestScenario(new Lexer(reader));
    }
    
    private class LexerTestScenario
    {
        private readonly Lexer lexer;
        private Token? lastToken;

        public LexerTestScenario(Lexer lexer) => this.lexer = lexer;
        
        public LexerTestScenario Then => this;
        public LexerTestScenario And => this;

        public LexerTestScenario NextToken
        {
            get {
            lastToken = lexer.Next();
            return this;
            }
        }

        public LexerTestScenario IsOfType(TokenType type)
        {
            Assert.That(lastToken, Is.Not.Null);
            Assert.That(lastToken.Type, Is.EqualTo(type));
            return this;
        }
        
        public LexerTestScenario HasValue(String value)
        {
            Assert.That(lastToken, Is.Not.Null);
            Assert.That(lastToken.Literal, Is.EqualTo(value));
            return this;
        }
    }
}