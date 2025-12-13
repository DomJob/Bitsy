using Bitsy.Lexing;

namespace Bitsy.Parsing;

public class Parser
{
    private static List<TokenType> ExpressionTokenTypes = [TokenType.Identifier, TokenType.End];

    public Expression ParseExpression(Queue<Token> tokens, int minBindingPower = 0, List<TokenType>? possibleEnds = null)
    {
        if(possibleEnds is null) possibleEnds = [TokenType.End];

        var token = tokens.Dequeue();
        var left = ParsePrefix(token, tokens, possibleEnds);

        while (tokens.Count > 0)
        {
            var op = tokens.Peek();
            if (possibleEnds.Contains(op.Type)) break;

            if (op.Type == TokenType.LeftParenthesis)
            {
                var (leftBp, _) = InfixBindingPower(op);
                if (leftBp < minBindingPower) break;
            
                tokens.Dequeue(); // consume '('
                var args = ParseExpressions(tokens, TokenType.RightParenthesis);
                tokens.Dequeue(); // consume ')'
                left = new CallExpression(left, args);
            }
            else if (op.Type == TokenType.Question)
            {
                var (leftBp, _) = InfixBindingPower(op);
                if (leftBp < minBindingPower) break;
                
                tokens.Dequeue(); // consume '?'
                var whenTrue = ParseExpression(tokens, 0, [TokenType.Colon]);
                tokens.Dequeue(); // consume ':'
                var whenFalse = ParseExpression(tokens, 0, possibleEnds);
                left = new ConditionalExpression(left, whenTrue, whenFalse);
            }
            else
            {
                var (leftBp, rightBp) = InfixBindingPower(op);
                if (leftBp < minBindingPower) break;
        
                tokens.Dequeue();
                var right = ParseExpression(tokens, rightBp, possibleEnds);
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

    private Expression ParsePrefix(Token token, Queue<Token> tokens, List<TokenType> possibleEnds)
    {
        if (IsPrefixOperator(token))
        {
            var bp = PrefixBindingPower(token);
            var operand = ParseExpression(tokens, bp, possibleEnds);
            return new PrefixExpression(token, operand);
        }
        
        if (token.Type == TokenType.LeftParenthesis)
        {
            var expr = ParseExpression(tokens, 0, [TokenType.RightParenthesis]);
            tokens.Dequeue();
            return expr;
        }

        if (token.Type == TokenType.LeftBracket)
        {
            var args = ParseExpressions(tokens, TokenType.RightBracket);
            tokens.Dequeue();
            return new ObjectExpression(args);
        }

        if (token.Type != TokenType.Identifier)
            throw new ParserException("Expected identifier", token);
    
        return new IdentifierExpression(token); 
    }
    
    private List<Expression> ParseExpressions(Queue<Token> tokens, TokenType endSignal)
    {
        var args = new List<Expression>();
    
        if (tokens.Peek().Type == endSignal)
            return args;
    
        args.Add(ParseExpression(tokens, 0, [endSignal, TokenType.Comma]));
    
        while (tokens.Peek().Type == TokenType.Comma)
        {
            tokens.Dequeue(); // consume ','
            args.Add(ParseExpression(tokens, 0, [endSignal, TokenType.Comma]));
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