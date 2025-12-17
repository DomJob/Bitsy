namespace Bitsy.Evaluating.Types;

public class BitType : Type
{
    public Bit bit;
    
    public BitType(Bit bit)
    {
        this.bit = bit;
    }

    public override List<Bit> ToBits()
    {
        return [bit];
    }
}