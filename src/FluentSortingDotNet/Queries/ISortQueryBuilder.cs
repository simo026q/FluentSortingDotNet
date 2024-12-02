using System.Linq.Expressions;

namespace FluentSortingDotNet.Queries;

public interface ISortQueryBuilder<T>
{
    bool IsEmpty { get; }
    void Reset();
    void SortBy(LambdaExpression expression, SortDirection direction);
    ISortQuery<T> Build();
}