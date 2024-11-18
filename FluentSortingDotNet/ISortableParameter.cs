using System.Linq.Expressions;

namespace FluentSortingDotNet;

public interface ISortableParameter<T>
{
    LambdaExpression Expression { get; }
    string Name { get; }
    bool IsRequired { get; }
    SortDirection? DefaultDirection { get; }
}
