using Bitsy.Typing.Types;

namespace Bitsy.Typing;

public abstract class Type
{
    public static bool AreEqual(Type left, Type right)
    {
        if (left is Unknown { inferredType: null } uLeft) uLeft.inferredType = right;
        if (right is Unknown { inferredType: null } uRight) uRight.inferredType = left;
        
        return left.Equals(right);
    }
    
    public abstract bool Equals(Type other);
}