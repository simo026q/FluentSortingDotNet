using System.Linq;

namespace FluentSortingDotNet.Queries;

internal sealed class NullSortQuery<T> : ISortQuery<T>
{
    public static NullSortQuery<T> Instance { get; } = new NullSortQuery<T>();
    private NullSortQuery()
    {
    }
    public IQueryable<T> Apply(IQueryable<T> query) => query;
}