namespace Bitsy.Analyzing.Types;

public record Field
{
    public string Name;
    public Type Type;

    public Field(string name, Type type)
    {
        Name = name;
        Type = type;
    }
}

public class Struct : Type
{
    public Struct(string name, List<Field> fields)
    {
        Name = name;
        Fields = fields;
    }

    public string Name { get; }

    public List<Field> Fields { get; }

    public override bool Equals(Type other)
    {
        return other is Struct o && Name == o.Name && Fields.SequenceEqual(o.Fields);
    }

    public override string ToString()
    {
        return Name;
    }
}