namespace Bitsy.Typing.Types;

public class Bit : Type
{
    public static readonly Bit Instance = new();

    private Bit()
    {
    }

    public override bool Equals(Type other)
    {
        return other is Bit;
    }

    public override string ToString()
    {
        return "Bit";
    }
}