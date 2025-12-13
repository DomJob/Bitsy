namespace Bitsy.Lexing;

public enum TokenType
{
    Identifier, Assignment,
    LeftParenthesis, RightParenthesis,  // ()
    LeftBracket, RightBracket,          // []
    LeftBrace, RightBrace,              // {}
    LeftAngle, RightAngle,              // <>
    Comma, Dot, Arrow,                  // , . ->
    Return, As,                         // Only textual keywords
    And, Or, Xor, Not,                  // Bit operations
    Equality, Question, Colon,          // Conditional branching
    Illegal, End,                       // Meta
}