namespace Bitsy.Analyzing.Types;

public class Function : Type
{
    public Type Input { get; }
    public Type Output { get; }

    public Function(Type input, Type output)
    {
        Input = input;
        Output = output;
    }

    public override bool Equals(Type other) => other is Function f && Input == f.Input && Output == f.Output;
}