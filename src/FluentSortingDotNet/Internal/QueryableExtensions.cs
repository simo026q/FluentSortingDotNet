using System.Linq.Expressions;
using System.Linq;
using System.Reflection;
using FluentSortingDotNet.Internal;

namespace FluentSortingDotNet.Internal;

internal static class QueryableExtensions
{
    private static readonly MethodInfo OrderByMethod;
    private static readonly MethodInfo OrderByDescendingMethod;
    private static readonly MethodInfo ThenByMethod;
    private static readonly MethodInfo ThenByDescendingMethod;

    static QueryableExtensions()
    {
        MethodInfo[] methods = typeof(Queryable).GetMethods(BindingFlags.Static | BindingFlags.Public);

        OrderByMethod = methods.First(x => x.Name == nameof(Queryable.OrderBy) && x.GetParameters().Length == 2);
        OrderByDescendingMethod = methods.First(x => x.Name == nameof(Queryable.OrderByDescending) && x.GetParameters().Length == 2);
        ThenByMethod = methods.First(x => x.Name == nameof(Queryable.ThenBy) && x.GetParameters().Length == 2);
        ThenByDescendingMethod = methods.First(x => x.Name == nameof(Queryable.ThenByDescending) && x.GetParameters().Length == 2);
    }

    private static IOrderedQueryable<T> CreateOrderQuery<T>(IQueryable<T> query, LambdaExpression expression, MethodInfo method)
    {
        return (IOrderedQueryable<T>)query.Provider.CreateQuery<T>(Expression.Call(
            null,
            method.MakeGenericMethod(typeof(T), expression.ReturnType),
            query.Expression,
            Expression.Quote(expression)));
    }

    public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> queryable, LambdaExpression expression)
        => CreateOrderQuery(queryable, expression, OrderByMethod);

    public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> queryable, LambdaExpression expression)
        => CreateOrderQuery(queryable, expression, OrderByDescendingMethod);

    public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> queryable, LambdaExpression expression)
        => CreateOrderQuery(queryable, expression, ThenByMethod);

    public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> queryable, LambdaExpression expression)
        => CreateOrderQuery(queryable, expression, ThenByDescendingMethod);
}
