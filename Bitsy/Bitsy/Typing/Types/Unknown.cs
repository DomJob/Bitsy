namespace Bitsy.Typing.Types;

public class Unknown : Type
{
    public static Unknown Instance = new();

    private Unknown()
    {
    }

    public override bool Equals(Type other)
    {
        return other is Unknown;
    }

    public override string ToString()
    {
        return "UNKNOWN";
    }
}