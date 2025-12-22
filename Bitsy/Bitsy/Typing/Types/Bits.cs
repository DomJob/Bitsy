namespace Bitsy.Typing.Types;

public class Bits : Type
{
    public static readonly Bits Instance = new();

    private Bits()
    {
    }

    public override bool Equals(Type other)
    {
        return other is Bits;
    }

    public override string ToString()
    {
        return "Bits";
    }
}