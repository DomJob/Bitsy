using System.Linq.Expressions;

namespace Bitsy.Evaluating;

public class Symbol
{
    public String Name { get; set; }
    public Expression Type { get; }

    public List<Bit> ToBits() => [];
    
    
}