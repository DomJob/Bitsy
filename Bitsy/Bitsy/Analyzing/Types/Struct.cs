namespace Bitsy.Analyzing.Types;

public record Field
{
    public string Name;
    public Type Type;

    public Field()
    {
    }

    public Field(string name, Type type)
    {
        Name = name;
        Type = type;
    }
}

public class Struct : Type
{
    public string Name { get; }
    
    public List<Field> Fields { get; }

    public Struct(string name, List<Field> fields)
    {
        Name = name;
        Fields = fields;
    }

    public override bool Equals(Type other) => other is Struct o && Name == o.Name && Fields.SequenceEqual(o.Fields);
    
    public override string ToString() => Name;
}