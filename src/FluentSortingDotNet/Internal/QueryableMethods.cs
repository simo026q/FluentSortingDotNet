using System.Linq;
using System.Reflection;
using FluentSortingDotNet.Internal;

namespace FluentSortingDotNet.Internal;

internal static class QueryableMethods
{
    public static readonly MethodInfo OrderBy;
    public static readonly MethodInfo OrderByDescending;
    public static readonly MethodInfo ThenBy;
    public static readonly MethodInfo ThenByDescending;

    static QueryableMethods()
    {
        var methods = typeof(Queryable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .GroupBy(x => x.Name)
            .ToDictionary(x => x.Key, x => x.ToList());

        OrderBy = GetMethod(nameof(Queryable.OrderBy), 2);
        OrderByDescending = GetMethod(nameof(Queryable.OrderByDescending), 2);
        ThenBy = GetMethod(nameof(Queryable.ThenBy), 2);
        ThenByDescending = GetMethod(nameof(Queryable.ThenByDescending), 2);

        MethodInfo GetMethod(string name, int parameterCount)
        {
            return methods[name].First(x => x.GetParameters().Length == parameterCount);
        }
    }

    public static MethodInfo GetOrderByMethod(SortDirection direction)
    {
        return direction == SortDirection.Ascending 
            ? OrderBy 
            : OrderByDescending;
    }

    public static MethodInfo GetThenByMethod(SortDirection direction)
    {
        return direction == SortDirection.Ascending
            ? ThenBy
            : ThenByDescending;
    }
}
