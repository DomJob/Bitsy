namespace Bitsy.Analyzing.Types;

public class Function : Type
{
    public Function(Type input, Type output)
    {
        Input = input;
        Output = output;
    }

    public Type Input { get; }
    public Type Output { get; }

    public override bool Equals(Type other)
    {
        return other is Function f && Input == f.Input && Output == f.Output;
    }
    
    public override string ToString() => $"({Input}->{Output})";
}