using System.Linq.Expressions;

namespace FluentSortingDotNet.Internal;

internal sealed class SortableParameter(LambdaExpression expression, string name)
{
    public LambdaExpression Expression { get; } = expression;
    public string Name { get; set; } = name;
    public SortDirection? DefaultDirection { get; set; }
    public bool ShouldReverseDirection { get; set; }
}