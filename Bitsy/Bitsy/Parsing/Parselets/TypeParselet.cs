using Bitsy.Lexing;
using Bitsy.Parsing.Expressions;

namespace Bitsy.Parsing.Parselets;

public class TypeParselet : InfixParselet
{
    public int Precedence => BindingPower.TypeDeclaration;
    
    public Expression Parse(Parser parser, Expression left, Token token)
    {
        return token.Type == TokenType.LeftBrace ? 
            ParsePlainType(parser, left) : 
            ParseTemplatedType(parser, left);
    }
    
    private TypeDeclaration ParsePlainType(Parser parser, Expression typeName)
    {
        
        
        return new TypeDeclaration(typeName, ParseBody(parser));
    }

    private List<(Expression, Expression)> ParseBody(Parser parser)
    {
        List<(Expression, Expression)> body = new List<(Expression, Expression)>();

        while (!parser.Match(TokenType.RightBrace))
        {
            var type = parser.ParseExpression();
            var name = parser.ParseExpression();
            body.Add((type, name));
        }

        return body;
    }

    private Expression ParseTemplatedType(Parser parser, Expression name)
    {
        List<Expression> templates = [parser.ParseExpression()];
        while (parser.Match(TokenType.Comma))
            templates.Add(parser.ParseExpression());
        parser.Consume(TokenType.RightAngle);

        if (parser.Match(TokenType.LeftBrace))
            return new TypeDeclaration(name, ParseBody(parser), templates);

        if (name is NameExpression nameExpr)
        {
            nameExpr.Templates = templates;
            return nameExpr;
        }
        throw new ParserException("Invalid use of Templating");
    }
}