using System.Linq.Expressions;
using System.Linq;
using System.Reflection;

namespace FluentSortingDotNet.Internal;

internal static class QueryableExtensions
{
    private static IOrderedQueryable<T> CreateOrderQuery<T>(IQueryable<T> query, LambdaExpression expression, MethodInfo method)
    {
        return (IOrderedQueryable<T>)query.Provider.CreateQuery<T>(
            QueryableHelper.CreateMethodCall(method.MakeGenericMethod(typeof(T), expression.ReturnType), query.Expression, expression));
    }

    public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> queryable, LambdaExpression expression)
        => CreateOrderQuery(queryable, expression, QueryableMethods.OrderBy);

    public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> queryable, LambdaExpression expression)
        => CreateOrderQuery(queryable, expression, QueryableMethods.OrderByDescending);

    public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> queryable, LambdaExpression expression)
        => CreateOrderQuery(queryable, expression, QueryableMethods.ThenBy);

    public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> queryable, LambdaExpression expression)
        => CreateOrderQuery(queryable, expression, QueryableMethods.ThenByDescending);
}
