namespace Bitsy.Typing.Types;

public class Function : Type
{
    public Function(Type input, Type output)
    {
        Input = input;
        Output = output;
    }

    public Type Input { get; }
    public Type Output { get; set; }

    public int ArgumentCount => Input is Union u ? u.Types.Count : 1;

    public override bool Equals(Type other)
    {
        return other is Function f && Input == f.Input && Output == f.Output;
    }

    public override string ToString()
    {
        return $"({Input}->{Output})";
    }

    public Type GetArg(int i)
    {
        return Input is Union u ? u.Types[i] : Input;
    }
}