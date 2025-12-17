using Bitsy.Parsing;
using Bitsy.Parsing.Expressions;

namespace Bitsy.Evaluating;

public class Environment
{
    public Type ResolveType(Expression expression)
    {
        throw new NotImplementedException();
    }

    public void Load(Parser parser)
    {
        throw new NotImplementedException();
    }

    public Type Evaluate(Expression expression)
    {
        throw new NotImplementedException();
    }
}