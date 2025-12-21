using Bitsy.Analyzing.Types;
using Bitsy.Lexing;
using Bitsy.Parsing;
using Bitsy.Parsing.Expressions;

namespace Bitsy.Analyzing;

public class TypeAnalyzer
{
    private TypeAnalyzer? parent = null;
    
    private TypeAnalyzer(TypeAnalyzer parent) => this.parent = parent;

    public TypeAnalyzer()
    {
        // typeEnv.RegisterType(new NameExpression("Bit"), Bit.Instance);
        // typeEnv.RegisterType(new NameExpression("Bits"), Bits.Instance);
        // typeEnv.BindTypeToSymbol(new NameExpression("0"), Bit.Instance);
        // typeEnv.BindTypeToSymbol(new NameExpression("1"), Bit.Instance);
    }

    public void LoadExpression(Expression expression)
    {
        // Constant binding
        
        // Function declaration
        
        // Type declaration
    }

    public Type ResolveType(Expression expression)
    {
        return null;
    }

    private Type ExpressionToType(Expression expression)
    {
        // NameExpression / SimpleTypeExpression
        
        // FunctionTypeExpression
        
        // UnionTypeExpression
        return null;
    }
    
    
}