namespace Bitsy.Parsing.Expressions;

public class TypeExpression : Expression
{
    public List<Expression> Inputs { get; }
    
    public Expression Output { get; }
    
    public TypeExpression(List<Expression> inputs, Expression output)
    {
        Inputs = inputs;
        Output = output;
    }
    
    public override string ToString()
    {
        if (Inputs.Count == 0) return $"()->{Output}";
        if (Inputs.Count == 1) return $"{Inputs[0]}->{Output}";
        return $"({string.Join(", ", Inputs.Select(e => e.ToString()))})->{Output}";
    }
}