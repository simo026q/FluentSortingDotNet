using System;
using System.Linq.Expressions;

namespace FluentSortingDotNet.Internal;

internal sealed class SortableParameter<T, TProperty>(Expression<Func<T, TProperty>> expression) : ISortableParameter<T>
{
    public LambdaExpression Expression { get; } = expression;
    public bool IsRequired { get; set; }
    public string Name { get; set; } = expression.Body.ToString();
    public SortDirection? DefaultDirection { get; set; }
}
