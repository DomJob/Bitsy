using Bitsy.Lexing;

namespace Bitsy.Parsing;

public class Parser
{
    private readonly Lexer lexer;

    private Token Peek() => lexer.Peek();
    private Token Next() => lexer.Next();

    public Parser(Lexer lexer)
    {
        this.lexer = lexer;
    }
    
    public Expression ParseExpression(int minBindingPower = 0, List<TokenType>? possibleEnds = null)
    {
        if(possibleEnds is null) possibleEnds = [TokenType.End];

        var token = Next();
        var left = ParsePrefix(token, possibleEnds);

        while (true)
        {
            var op = Peek();
            if (possibleEnds.Contains(op.Type)) break;

            if (op.Type == TokenType.LeftParenthesis)
            {
                var (leftBp, _) = InfixBindingPower(op);
                if (leftBp < minBindingPower) break;
            
                Next(); // consume '('
                var args = ParseExpressions(TokenType.RightParenthesis);
                Next(); // consume ')'
                left = new CallExpression(left, args);
            }
            else if (op.Type == TokenType.Question)
            {
                var (leftBp, _) = InfixBindingPower(op);
                if (leftBp < minBindingPower) break;
                
                Next(); // consume '?'
                var whenTrue = ParseExpression(0, [TokenType.Colon]);
                Next(); // consume ':'
                var whenFalse = ParseExpression(0, possibleEnds);
                left = new ConditionalExpression(left, whenTrue, whenFalse);
            }
            else
            {
                var (leftBp, rightBp) = InfixBindingPower(op);
                if (leftBp < minBindingPower) break;
        
                Next();
                var right = ParseExpression(rightBp, possibleEnds);
                left = new InfixExpression(left, op, right);
            }

            //throw new ParserException($"Unexpected token. Expected one of {string.Join(", ",possibleEnds)}", token);
        }
        return left;
    }

    private (int leftBp, int rightBp) InfixBindingPower(Token operation)
    {
        switch (operation.Type)
        {
            case TokenType.LeftParenthesis: return (16, 17);
            case TokenType.Dot: return (14, 15);
            case TokenType.And: return (12, 13);
            case TokenType.Xor: return (10, 11);
            case TokenType.Or: return (8, 9);
            case TokenType.Question: return (7, 6);
        }
        throw new ParserException("Expected InfixOperator token", operation);
    }

    private Expression ParsePrefix(Token token, List<TokenType> possibleEnds)
    {
        if (IsPrefixOperator(token))
        {
            var bp = PrefixBindingPower(token);
            var operand = ParseExpression(bp, possibleEnds);
            return new PrefixExpression(token, operand);
        }
        
        if (token.Type == TokenType.LeftParenthesis)
        {
            var expr = ParseExpression(0, [TokenType.RightParenthesis]);
            Next();
            return expr;
        }

        if (token.Type == TokenType.LeftBracket)
        {
            var args = ParseExpressions(TokenType.RightBracket);
            Next();
            return new ObjectExpression(args);
        }

        if (token.Type != TokenType.Identifier)
            throw new ParserException("Expected identifier", token);
    
        return new IdentifierExpression(token); 
    }
    
    private List<Expression> ParseExpressions(TokenType endSignal)
    {
        var args = new List<Expression>();
    
        if (Peek().Type == endSignal)
            return args;
    
        args.Add(ParseExpression(0, [endSignal, TokenType.Comma]));
    
        while (Peek().Type == TokenType.Comma)
        {
            Next(); // consume ','
            args.Add(ParseExpression(0, [endSignal, TokenType.Comma]));
        }
    
        return args;
    }


    private int PrefixBindingPower(Token token)
    {
        return 14;
    }

    private bool IsPrefixOperator(Token token)
    {
        return token.Type == TokenType.Not;
    }
}

public class ParserException : Exception
{
    public string Message { get; }
    public Token? Token { get; }
    public ParserException(string message, Token token)
    {
        Message = message;
        Token = token;
    }

    public ParserException(string message)
    {
        Message = message;
    }
}