using Bitsy.Lexing;
using Bitsy.Parsing.Expressions;

namespace Bitsy.Parsing.Parselets;

public class GroupParselet : PrefixParselet
{
    public Expression Parse(Parser parser, Token token)
    {
        if (parser.Match(TokenType.RightParenthesis))
        {
            return ParseTypeWithNoInput(parser);
        }
        
        var expression = parser.ParseExpression();

        if (parser.Match(TokenType.RightParenthesis))
        {
            if (!parser.Match(TokenType.Arrow))
                return expression;
            
            return ParseTypeWithOneInput(parser, expression);
        }

        List<Expression> inputs = [expression];
        parser.Consume(TokenType.Comma);
        while(!parser.Match(TokenType.RightParenthesis))
            inputs.Add(parser.ParseExpression());
        parser.Consume(TokenType.Arrow);
        return new TypeName(inputs, parser.ParseExpression());
    }

    private TypeName ParseTypeWithOneInput(Parser parser, Expression expression)
    {
        return new TypeName([], parser.ParseExpression());
    }

    private TypeName ParseTypeWithNoInput(Parser parser)
    {
        parser.Consume(TokenType.Arrow);
        var output = parser.ParseExpression();

        return new TypeName([], output);
    }


    public int Precedence { get; }
}