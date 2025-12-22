namespace Bitsy.Analyzing.Types;

public class Bits : Type
{
    public static Bits Instance = new();

    private Bits()
    {
    }

    public override bool Equals(Type other)
    {
        return other is Bits;
    }
    
    public override string ToString() => "Bits";
}