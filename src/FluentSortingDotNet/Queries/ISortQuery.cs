using System.Linq;

namespace FluentSortingDotNet.Queries;

/// <summary>
/// Represents a query for sorting.
/// </summary>
/// <typeparam name="T">The type of items to sort.</typeparam>
public interface ISortQuery<T>
{
    /// <summary>
    /// Applies the sorting to the specified query.
    /// </summary>
    /// <param name="query">The query to apply the sorting to.</param>
    /// <returns>A new query with the sorting applied.</returns>
    IQueryable<T> Apply(IQueryable<T> query);
}