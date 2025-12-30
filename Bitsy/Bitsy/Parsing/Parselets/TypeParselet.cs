using Bitsy.Lexing;
using Bitsy.Parsing.Expressions;

namespace Bitsy.Parsing.Parselets;

public class TypeParselet : PrefixParselet
{
    public int Precedence { get; }

    public Expression Parse(Parser parser, Token token)
    {
        var templates = parser.Match(TokenType.LeftAngle) ? ParseTemplates(parser) : [];
        TypeExpression left = new SimpleTypeExpression(token, templates);
        while (parser.Match(TokenType.Arrow))
        {
            var right = parser.ParseExpression();
            if (right is TypeExpression type)
                left = new FunctionTypeExpression(left, type);
            if (parser.Match(TokenType.RightParenthesis)) break;
        }

        if (!parser.Match(TokenType.LeftBrace))
            return left;
        if (left is not SimpleTypeExpression simple)
            throw new ParserException("Expected simple type, got " + left);
        return new TypeDeclaration(simple, ParseBody(parser));
    }

    private List<(TypeExpression, NameExpression)> ParseBody(Parser parser)
    {
        List<(TypeExpression Type, NameExpression Name)> definition = [];

        while (true)
        {
            if (parser.Match(TokenType.RightBrace)) break;
            var expr1 = parser.ParseExpression();
            var expr2 = parser.ParseExpression();

            if (expr1 is not TypeExpression signature)
                throw new ParserException($"Expected type signature, got {expr1}");
            if (expr2 is not NameExpression name)
                throw new ParserException($"Expected identifier, got {expr2}");
            definition.Add((signature, name));
        }

        return definition;
    }

    private List<TypeExpression> ParseTemplates(Parser parser)
    {
        List<TypeExpression> templates = [];
        do
        {
            var expr = parser.ParseExpression();
            if (expr is not TypeExpression templateType)
                throw new ParserException($"Expected a type, got {expr}");
            templates.Add(templateType);
            parser.Match(TokenType.Comma);
        } while (!parser.Match(TokenType.RightAngle));

        return templates;
    }
}