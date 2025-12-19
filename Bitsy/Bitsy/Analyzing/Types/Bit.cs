namespace Bitsy.Analyzing.Types;

public class Bit : Type
{
    public static Bit Instance = new Bit();

    private Bit()
    {
    }

    public override bool Equals(Type other) => other is Bit;
}