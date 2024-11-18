using System.Linq.Expressions;

namespace FluentSortingDotNet.Internal;

internal interface ISortableParameter<T>
{
    LambdaExpression Expression { get; }
    string Name { get; }
    SortDirection? DefaultDirection { get; }
}
