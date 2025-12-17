namespace Bitsy.Evaluating.Types;

public class StructureType : Type
{
    public string Name { get; }
    
    public Dictionary<string, Type> Attributes { get; }

    public StructureType(string name, Dictionary<string, Type> attributes)
    {
        Name = name;
        Attributes = attributes;
    }

    public override List<Bit> ToBits()
    {
        List<Bit> bits = new List<Bit>();
        foreach (var attr in Attributes.Values)
        {
            bits.AddRange(attr.ToBits());
        }

        return bits;
    }
}