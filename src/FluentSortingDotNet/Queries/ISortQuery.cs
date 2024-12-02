using System.Linq;

namespace FluentSortingDotNet.Queries;

public interface ISortQuery<T>
{
    IOrderedQueryable<T> Apply(IQueryable<T> query);
}
