using Bitsy.Analyzing.Types;
using Bitsy.Lexing;
using Bitsy.Parsing;
using Bitsy.Parsing.Expressions;

namespace Bitsy.Analyzing;

public class Analyzer
{
    private readonly Dictionary<string, Type> symbolTypes = new();
    private readonly Dictionary<string, Type> typEnv = new();

    public Analyzer()
    {
        RegisterType(new NameExpression("Bit"), Bit.Instance);
        RegisterSymbol(new NameExpression("0"), Bit.Instance);
        RegisterSymbol(new NameExpression("1"), Bit.Instance);
    }
    
    public void LoadExpression(Expression expression)
    {
        switch (expression)
        {
            case BinaryExpression { Operation.Type: TokenType.Assignment } op:
                var name = op.Left.ToString();
                symbolTypes[name!] = ResolveType(op.Right);
                break;
            case FunctionDeclaration func:
                LoadFunctionDeclaration(func);
                break;
        }
    }

    private void LoadFunctionDeclaration(FunctionDeclaration func)
    {
        var input = func.Args.Count == 1 ?
            ResolveType(func.Args[0].Item1)
            : new Union(func.Args.Select(a => ResolveType(a.Item1)).ToList());

        var returnStatement = func.Body.FirstOrDefault(expr => expr is UnaryExpression { Operation.Type: TokenType.Return });
        
        var output = returnStatement == null ? new Union([]) : ResolveType(((UnaryExpression)returnStatement).Operand);

        RegisterSymbol(func.Name, new Function(input, output));
    }

    public Type ResolveType(Expression expression)
    {
        switch (expression)
        {
            case NameExpression name:
                return LookupSymbol(name);
            case SimpleTypeExpression type:
                return typEnv[type.Literal];
            case BinaryExpression { Operation.Type: TokenType.As } expr:
                ValidateTypeExists(expr.Right);
                return typEnv[expr.Right.Literal];
            case BinaryExpression expr:
                ValidateBitType(expr.Right);
                ValidateBitType(expr.Left);
                return Bit.Instance;
            case FunctionDeclaration func:
                return ResolveType(func.Name);
        }
        throw new TypeException($"Unknown expression type '{expression.GetType().Name}'");
    }

    private void ValidateTypeExists(Expression expression)
    {
        if (!typEnv.ContainsKey(expression.Literal))
            throw new UnknownTypeException($"Unknown type '{expression}'");
    }

    private void ValidateBitType(Expression expr)
    {
        if(LookupSymbol(expr) != Bit.Instance)
            throw new WrongTypeException($"Expression type '{expr.GetType().Name}' is not a bit");
    }

    private Type LookupSymbol(Expression symbol)
    {
        if(!symbolTypes.TryGetValue(symbol.Literal, out var type)) throw new UnknownSymbolException($"Unknown symbol '{symbol}'");
        return type;
    }
    private void RegisterSymbol(Expression symbol, Type type) => symbolTypes[symbol.Literal] = type;

    private Type ResolveTypeName(Expression typeName)
    {
        if(!typEnv.TryGetValue(typeName.Literal, out var type)) throw new UnknownTypeException($"Unknown symbol '{typeName}'");
        return type;
    }
    private void RegisterType(Expression typeName, Type type) => typEnv[typeName.Literal] = type;
}