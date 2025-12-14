using Bitsy.Parsing.Expressions;

namespace Bitsy.Parsing.Statements;

public class BindingStatement : Statement
{
    public IdentifierExpression Identifier { get; set; }
    
    public Expression Expression { get; set; }

    public BindingStatement(Expression identifier, Expression expression)
    {
        Identifier = identifier as IdentifierExpression ?? throw new ParserException("Expected identifier expression");
        Expression = expression;
    }

    public override string ToString() => $"{Identifier} = {Expression}";
}