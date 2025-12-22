using Bitsy.Lexing;
using Bitsy.Parsing.Expressions;

namespace Bitsy.Parsing.Parselets;

public class LeftParenthesisParselet : InfixParselet
{
    public Expression Parse(Parser parser, Expression left, Token token)
    {
        if (parser.Match(TokenType.RightParenthesis))
        {
            if (parser.Match(TokenType.LeftBrace))
                return new FunctionDeclaration(ToNameExpression(left), [], ParseFunctionBody(parser));

            return new CallExpression(left, []);
        }

        var nextExpr = parser.ParseExpression();

        if (nextExpr is TypeExpression typeExpr)
            return ParseFunctionDeclaration(parser, ToNameExpression(left), typeExpr);

        List<Expression> callArgs = [nextExpr];

        while (parser.Match(TokenType.Comma)) callArgs.Add(parser.ParseExpression());

        parser.Consume(TokenType.RightParenthesis);

        return new CallExpression(left, callArgs);
    }

    public int Precedence => BindingPower.Call;

    private FunctionDeclaration ParseFunctionDeclaration(Parser parser, NameExpression name, TypeExpression firstType)
    {
        List<(TypeExpression, NameExpression)> args = [(firstType, parser.ParseName())];

        while (parser.Match(TokenType.Comma)) args.Add((parser.ParseTypeSignature(), parser.ParseName()));
        parser.Consume(TokenType.RightParenthesis);
        parser.Consume(TokenType.LeftBrace);

        return new FunctionDeclaration(name, args, ParseFunctionBody(parser));
    }

    private List<Expression> ParseFunctionBody(Parser parser)
    {
        List<Expression> body = [];
        while (!parser.Match(TokenType.RightBrace))
            body.Add(parser.ParseExpression());
        parser.Match(TokenType.RightBrace);
        return body;
    }


    private NameExpression ToNameExpression(Expression expr)
    {
        if (expr is NameExpression nameExpr)
            return nameExpr;
        throw new ParserException("Expected name expression in function declaration");
    }
}