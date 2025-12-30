using System.ComponentModel.DataAnnotations;

namespace Bitsy.Typing.Types;

public record Field
{
    public readonly string Name;
    public Type Type;
    
    public Field(string name, Type type)
    {
        Name = name;
        Type = type;
    }

    public bool CheckEqual(Field other)
    {
        return other.Name == Name && other.Type.Equals(Type);
    }
}

public class Struct : Type
{
    public Struct(string name, List<Field> fields)
    {
        Name = name;
        Fields = fields;
    }

    public List<Type> Templates = [];
    
    public string Name { get; }

    public List<Field> Fields { get; }

    public bool FieldsEqual(List<Field> otherFields)
    {
        if (Fields.Count != otherFields.Count)
            return false;
        
        for(var i = 0; i < Fields.Count; i++)
            if(!Type.AreEqual(Fields[i].Type, otherFields[i].Type))
                return false;
        return true;
    }

    public override bool Equals(Type other)
    {
        if (other is not Struct o || Name == o.Name)
            return false;

        return FieldsEqual(o.Fields);
    }

    public override string ToString()
    {
        return Name + (Templates.Count > 0 ? "<" + string.Join(", ", Templates)+">" : "");
    }
}