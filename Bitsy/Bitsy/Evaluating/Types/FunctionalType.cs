namespace Bitsy.Evaluating.Types;

public class FunctionalType : Type
{
    public Type? Input { get; }
    public Type? Output { get; }

    public FunctionalType(Type? input, Type? output)
    {
        Input = input;
        Output = output;
    }

    public override List<Bit> ToBits()
    {
        throw new NotImplementedException();
    }
}