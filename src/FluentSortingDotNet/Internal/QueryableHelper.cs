using System.Linq.Expressions;
using System.Reflection;

namespace FluentSortingDotNet.Internal;

internal static class QueryableHelper
{
    public static MethodCallExpression CreateMethodCall(MethodInfo method, Expression queryable, LambdaExpression expression)
    {
        return Expression.Call(
            null,
            method,
            queryable,
            Expression.Quote(expression));
    }
}