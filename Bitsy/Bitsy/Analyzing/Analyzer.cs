using Bitsy.Analyzing.Types;
using Bitsy.Lexing;
using Bitsy.Parsing;
using Bitsy.Parsing.Expressions;

namespace Bitsy.Analyzing;

public class Analyzer
{
    private Dictionary<string, Type> typeEnv = new();

    public Analyzer()
    {
        typeEnv["0"] = Bit.Instance;
        typeEnv["1"] = Bit.Instance;
    }
    
    public void LoadExpression(Expression expression)
    {
        switch (expression)
        {
            case BinaryExpression { Operation.Type: TokenType.Assignment } op:
                var name = op.Left.ToString();
                typeEnv[name!] = ResolveType(op.Right);
                break;
        }
    }
    
    public Type ResolveType(Expression expression)
    {
        switch (expression)
        {
            case NameExpression name:
                return LookupType(name);
            case BinaryExpression expr:
                ValidateBitType(expr.Right);
                ValidateBitType(expr.Left);
                return Bit.Instance;
        }
        throw new TypeException($"Unknown expression type '{expression.GetType().Name}'");
    }

    private void ValidateBitType(Expression expr)
    {
        if(LookupType(expr) != Bit.Instance)
            throw new WrongTypeException($"Expression type '{expr.GetType().Name}' is not a bit");
    }

    private Type LookupType(Expression symbol)
    {
        if(!typeEnv.TryGetValue(symbol.Literal, out var type)) throw new UnknownSymbolException($"Unknown symbol '{symbol}'");
        return type;
    }
}