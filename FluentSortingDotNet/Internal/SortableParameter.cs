using System;
using System.Linq.Expressions;

namespace FluentSortingDotNet.Internal;

internal sealed class SortableParameter<T, TProperty>(Expression<Func<T, TProperty>> expression) : ISortableParameter<T>
{
    public LambdaExpression Expression { get; } = expression;
    public bool IsRequired { get; set; }
    public string Name { get; set; } = GetNameFromExpression(expression);
    public SortDirection? DefaultDirection { get; set; }

    private static string GetNameFromExpression(Expression<Func<T, TProperty>> expression)
    {
        var bodyString = expression.Body.ToString();
        var prefix = expression.Parameters[0].Name + ".";

        return bodyString.StartsWith(prefix) 
            ? bodyString.Substring(prefix.Length) 
            : bodyString;
    }
}
