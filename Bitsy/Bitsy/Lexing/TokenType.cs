namespace Bitsy.Lexing;

public enum TokenType
{
    Identifier, Assignment, Space,      //
    LeftParenthesis, RightParenthesis,  // ()
    LeftBracket, RightBracket,          // []
    LeftBrace, RightBrace,              // {}
    LeftAngle, RightAngle,              // <>
    Comma, Dot, Arrow,                  // , . ->
    Return, As,                         // Only keywords
    And, Or, Xor, Not,                  // Bit operations
    Equality, Question, Colon,          // Conditional branching
    Illegal, End,                       // Meta
}