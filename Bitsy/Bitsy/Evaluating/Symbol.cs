using System.Linq.Expressions;

namespace Bitsy.Evaluating;

public class Symbol
{
    public string Name { get; set; }
    public Expression Type { get; }

    public List<Bit> ToBits()
    {
        return [];
    }
}