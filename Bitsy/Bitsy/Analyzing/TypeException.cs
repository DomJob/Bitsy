namespace Bitsy.Analyzing;

public class TypeException : Exception
{
    public TypeException(string message) : base(message)
    {
    }
}

public class UnknownSymbolException : Exception
{
    public UnknownSymbolException(string message) : base(message)
    {
    }
}

public class UnknownTypeException : Exception
{
    public UnknownTypeException(string message) : base(message)
    {
    }
}

public class WrongTypeException : Exception
{
    public WrongTypeException(string message) : base(message)
    {
    }
}

public class UnknownFieldException : Exception
{
    public UnknownFieldException(string message) : base(message)
    {
        
    }
}
