namespace Bitsy.Typing.Types;

public class Unknown : Type
{
    internal Type? inferredType = null;
    
    public Unknown()
    {
    }

    public override bool Equals(Type other)
    {
        return inferredType?.Equals(other) ?? true;
    }

    public override string ToString()
    {
        return inferredType == null ? "?" : inferredType.ToString()!;
    }
}