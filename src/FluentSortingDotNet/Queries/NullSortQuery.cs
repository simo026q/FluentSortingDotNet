using System.Linq;

namespace FluentSortingDotNet.Queries;

internal sealed class NullSortQuery<T> : ISortQuery<T>
{
    public readonly static NullSortQuery<T> Instance = new();

    private NullSortQuery()
    {
    }

    public IQueryable<T> Apply(IQueryable<T> query) => query;
}