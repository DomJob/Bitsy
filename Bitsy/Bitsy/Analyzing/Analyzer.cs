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
        }
        throw new TypeException($"Unknown expression type '{expression.GetType().Name}'");
    }

    private Type LookupType(Expression symbol)
    {
        if(!typeEnv.TryGetValue(symbol.Literal, out var type)) throw new TypeException($"Unknown symbol '{symbol}'");
        return type;
    }
}