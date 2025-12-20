namespace Bitsy.Analyzing.Types;

public class Union : Type
{
    public readonly List<Type> Types;

    public Union(List<Type> types)
    {
        Types = types;
    }

    public override bool Equals(Type other) => other is Union u && u.Types == this.Types;
}