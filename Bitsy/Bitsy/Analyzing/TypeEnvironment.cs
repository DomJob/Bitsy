using Bitsy.Analyzing.Types;
using Bitsy.Lexing;
using Bitsy.Parsing;
using Bitsy.Parsing.Expressions;

namespace Bitsy.Analyzing;

public class TypeEnvironment
{
    private TypeEnvironment? parent;

    private Dictionary<string, Type> availableTypes = new();
    private Dictionary<string, Type> knownSymbols = new();
    
    private TypeEnvironment(TypeEnvironment parent)
    {
        this.parent = parent;
    }

    public TypeEnvironment()
    {
        RegisterType("Bits", Bits.Instance);
        RegisterType("Bit", Bit.Instance);
        RegisterSymbol("0", Bit.Instance);
        RegisterSymbol("1", Bit.Instance);
    }

    public void ReadStatement(Expression expression)
    {
        switch (expression)
        {
            case AssignmentExpression assignment:
                knownSymbols[assignment.Name.Literal] = ResolveType(assignment.Expression);
                break;
            case FunctionDeclaration function:
                ReadFunctionDeclaration(function);
                break; // TODO
            case TypeDeclaration type:
                break; // TODO
            case ReturnExpression returnExpression:
                break;
            default:
                throw new TypeException("Expected a statement, got: " + expression);
        }
    }

    private void ReadFunctionDeclaration(FunctionDeclaration function)
    {
        var functionEnv = new TypeEnvironment(this);
        List<Type> inputs = [];
        
        foreach (var (typeExpr, nameExpr) in function.Args)
        {
            var typeInstance = ResolveTypeExpression(typeExpr);
            functionEnv.RegisterSymbol(nameExpr.Literal, typeInstance);
            inputs.Add(typeInstance);
        }

        Type returnType = new Union([]);
        foreach (var expr in function.Body)
        {
            functionEnv.ReadStatement(expr);
            if(expr is ReturnExpression returnExpr)
                returnType = functionEnv.ResolveType(returnExpr.Expression);
        }
        
        var functionType = new Function(inputs.Count == 1 ? inputs[0] : new Union(inputs), returnType);
        RegisterSymbol(function.Name.Literal, functionType); 
    }

    private Type ResolveTypeExpression(TypeExpression expression)
    {
        Type? foundType = null;
        
        switch (expression)
        {
            case SimpleTypeExpression simpleType:
                availableTypes.TryGetValue(simpleType.Literal, out foundType);
                break;
            case FunctionTypeExpression functionTypeExpression:
                // TODO
                break;
            case UnionTypeExpression unionTypeExpression:
                // TODO
                break;
        }
        
        if(foundType == null)
            return parent?.ResolveTypeExpression(expression) ?? throw new UnknownTypeException("Unknown type: " + expression);
        return foundType;
    }

    public Type ResolveType(Expression expression)
    {
        Type? foundType = null;
        
        switch (expression)
        {
            case NameExpression name:
                knownSymbols.TryGetValue(name.Name.Literal, out foundType);
                break;
            case BinaryExpression { Operation.Type: TokenType.As } casting:
                foundType = ResolveTypeExpression((TypeExpression)casting.Right);
                break;
            case BinaryExpression op:
                if(ResolveType(op.Left) != Bit.Instance)
                    throw new WrongTypeException("Expected expression of type Bit" + op.Left.Literal);
                if(ResolveType(op.Right) != Bit.Instance)
                    throw new WrongTypeException("Expected expression of type Bit" + op.Right.Literal);
                return Bit.Instance;
        }
        
        if(foundType == null)
            return parent?.ResolveType(expression) ?? throw new UnknownSymbolException("Unknown type for symbol: " + expression);
        return foundType;
    }
    
    private void RegisterType(string name, Type type) => availableTypes.Add(name, type);
    
    private void RegisterSymbol(string name, Type type) => knownSymbols.Add(name, type);
}