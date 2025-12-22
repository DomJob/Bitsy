namespace Bitsy.Typing;

public class TypeException : Exception
{
    public TypeException(string message) : base(message)
    {
    }
}

public class UnknownSymbolException : TypeException
{
    public UnknownSymbolException(string message) : base(message)
    {
    }
}

public class UnknownTypeException : TypeException
{
    public UnknownTypeException(string message) : base(message)
    {
    }
}

public class WrongTypeException : TypeException
{
    public WrongTypeException(string message) : base(message)
    {
    }
}

public class UnknownFieldException : TypeException
{
    public UnknownFieldException(string message) : base(message)
    {
    }
}

public class AmbiguousObjectTypeException : TypeException
{
    public AmbiguousObjectTypeException(string message) : base(message)
    {
    }
}

public class SymbolAlreadyDefinedException : TypeException
{
    public SymbolAlreadyDefinedException(string message) : base(message)
    {
    }
}

public class WrongCallArgumentException : TypeException
{
    public WrongCallArgumentException(string message) : base(message)
    {
    }
}