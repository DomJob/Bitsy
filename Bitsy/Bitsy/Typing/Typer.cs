using Bitsy.Analyzing.Types;
using Bitsy.Lexing;
using Bitsy.Parsing;
using Bitsy.Parsing.Expressions;

namespace Bitsy.Analyzing;

public class Typer
{
    private readonly Dictionary<string, Type> availableTypes = new();
    private readonly Dictionary<string, Type> knownSymbols = new();
    private readonly Typer? parent;
    private readonly List<Struct> structs = new();

    private Typer(Typer parent)
    {
        this.parent = parent;
    }

    public Typer()
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
                ReadAssignment(assignment);
                break;
            case FunctionDeclaration function:
                ReadFunctionDeclaration(function);
                break;
            case TypeDeclaration type:
                ReadTypeDeclaration(type);
                break;
            case ReturnExpression returnExpression:
                break;
            default:
                throw new TypeException("Expected a statement, got: " + expression);
        }
    }

    private void ReadAssignment(AssignmentExpression assignment)
    {
        if (!knownSymbols.TryAdd(assignment.Name.Literal, ResolveType(assignment.Expression)))
            throw new SymbolAlreadyDefinedException($"Symbol {assignment.Name.Literal} already defined in context");
    }

    private void ReadTypeDeclaration(TypeDeclaration type)
    {
        var newType = new Struct(type.Name.Literal, []);
        RegisterType(type.Name.Literal, newType);

        foreach (var (typeExpr, nameExpr) in type.Body)
        {
            var typeInstance = ResolveTypeExpression(typeExpr);
            newType.Fields.Add(new Field(nameExpr.Literal, typeInstance));
        }
    }

    private void ReadFunctionDeclaration(FunctionDeclaration function)
    {
        var functionEnv = new Typer(this);
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
            if (expr is ReturnExpression returnExpr)
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

        if (foundType == null)
            return parent?.ResolveTypeExpression(expression) ??
                   throw new UnknownTypeException("Unknown type: " + expression);
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
            case BinaryExpression { Left: ExplicitObjectExpression left, Operation.Type: TokenType.As } casting:
                foundType = ResolveTypeExpression((TypeExpression)casting.Right);
                ValidateStructFields(left, foundType);
                break;
            case BinaryExpression { Operation.Type: TokenType.As } casting:
                foundType = ResolveTypeExpression((TypeExpression)casting.Right);
                break;
            case BinaryExpression op:
                AssertTypeIs(op.Left, Bit.Instance);
                AssertTypeIs(op.Right, Bit.Instance);
                return Bit.Instance;
            case UnaryExpression unary:
                AssertTypeIs(unary.Operand, Bit.Instance);
                return Bit.Instance;
            case ConditionalExpression cond:
                AssertTypeIs(cond.Condition, Bit.Instance);
                var condType = ResolveType(cond.IfTrue);
                AssertTypeIs(cond.IfFalse, condType);
                return condType;
            case CallExpression call:
                return ResolveCallType(call);
            case ImplicitObjectExpression:
                return Bits.Instance;
            case ExplicitObjectExpression obj:
                return InferStruct(obj);
        }

        if (foundType == null)
            return parent?.ResolveType(expression) ??
                   throw new UnknownSymbolException("Unknown type for symbol: " + expression);
        return foundType;
    }

    private Type ResolveCallType(CallExpression call)
    {
        var targetType = ResolveType(call.Expression);
        if (targetType is not Function functionType)
            throw new WrongTypeException("Can't call non-function " + call.Expression);

        var argumentsUsed = call.Arguments.Count;
        var argumentsExpected = functionType.ArgumentCount;

        if (argumentsUsed != argumentsExpected)
            throw new WrongCallArgumentException(
                $"Target expression takes {argumentsUsed} arguments, trying to call with {argumentsExpected}");

        for (var i = 0; i < argumentsUsed; i++) AssertTypeIs(call.Arguments[i], functionType.GetArg(i));

        return functionType.Output;
    }

    private Struct InferStruct(ExplicitObjectExpression obj)
    {
        var possibleTargets = structs.Where(s => s.Fields.Count == obj.Body.Count).ToList();

        if (possibleTargets.Any())
        {
            var typedBody = obj.Body.Select(field => new Field(field.Item1.Literal, ResolveType(field.Item2))).ToList();
            possibleTargets = possibleTargets.Where(s => s.Fields.SequenceEqual(typedBody)).ToList();

            if (possibleTargets.Count == 1) return possibleTargets.First();
            if (possibleTargets.Count > 1)
                throw new AmbiguousObjectTypeException("Can't infer object type, too many options");
        }

        return parent?.InferStruct(obj) ?? throw new UnknownTypeException("Can't infer object type for " + obj);
    }

    private void AssertTypeIs(Expression expression, Type expectedType)
    {
        var actual = ResolveType(expression);
        if (actual != expectedType)
            throw new WrongTypeException("Expected expression of type " + expression.Literal + ", got " + actual);
    }

    private void ValidateStructFields(ExplicitObjectExpression obj, Type type)
    {
        if (type is not Struct objType)
            throw new WrongTypeException("Expected structured type, got: " + type);
        if (obj.Body.Count != objType.Fields.Count)
            throw new WrongTypeException("Wrong number of body args when instantiating object");

        var dict = new Dictionary<string, Type>();
        objType.Fields.ForEach(field => dict[field.Name] = field.Type);

        foreach (var (nameExpr, expr) in obj.Body)
        {
            dict.TryGetValue(nameExpr.Literal, out var expectedType);
            if (expectedType == null)
                throw new WrongTypeException($"Unknown field accessor {nameExpr.Literal}");
            var actualType = ResolveType(expr);

            if (expectedType != actualType)
                throw new WrongTypeException("Wrong type when resolving explicit object instantiation");
        }
    }

    private void RegisterType(string name, Type type)
    {
        if (type is Struct s) structs.Add(s);
        if (!availableTypes.TryAdd(name, type))
            throw new SymbolAlreadyDefinedException("Already defined type " + name);
    }

    private void RegisterSymbol(string name, Type type)
    {
        knownSymbols.Add(name, type);
    }
}