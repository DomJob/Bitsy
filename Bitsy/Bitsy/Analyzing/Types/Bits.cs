namespace Bitsy.Analyzing.Types;

public class Bits : Type
{
    public static Bits Instance = new Bits();

    private Bits()
    {
    }

    public override bool Equals(Type other) => other is Bits;
}