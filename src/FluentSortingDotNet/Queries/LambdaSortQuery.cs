using System;
using System.Linq;

namespace FluentSortingDotNet.Queries;

public readonly struct LambdaSortQuery<T>(Func<IQueryable<T>, IOrderedQueryable<T>> sortFunction) : ISortQuery<T>
{
    private readonly Func<IQueryable<T>, IOrderedQueryable<T>> _sortFunction = sortFunction;

    public IOrderedQueryable<T> Apply(IQueryable<T> query)
        => _sortFunction(query);
}