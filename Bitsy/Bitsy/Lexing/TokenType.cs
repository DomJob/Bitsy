namespace Bitsy.Lexing;

public enum TokenType
{
    Identifier,
    LeftParenthesis,
    RightParenthesis, // ()

    Assignment,
    LeftBracket,
    RightBracket, // []
    LeftBrace,
    RightBrace, // {}
    LeftAngle,
    RightAngle, // <>
    Comma,
    Dot,
    Arrow, // , . ->
    Return,
    As, // Only textual keywords
    And,
    Or,
    Xor,
    Not, // Bit operations
    Question,
    Colon, // Conditional branching
    Illegal,
    End // Meta
}