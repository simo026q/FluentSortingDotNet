using System.Linq;

namespace FluentSortingDotNet.Queries;

/// <summary>
/// Extension methods for the <see cref="ISortQueryBuilder{T}"/> interface.
/// </summary>
public static class SortQueryBuilder
{
    /// <summary>
    /// Builds the sort query and resets the query builder to its initial state.
    /// </summary>
    /// <typeparam name="T">The type of items to sort.</typeparam>
    /// <param name="builder">The query builder to build the query from.</param>
    /// <returns>Returns the built sort query.</returns>
    public static ISortQuery<T> BuildAndReset<T>(this ISortQueryBuilder<T> builder)
    {
        ISortQuery<T> query = builder.Build();
        builder.Reset();
        return query;
    }

    /// <summary>
    /// Applies the sorting to the specified query.
    /// </summary>
    /// <typeparam name="T">The type of items to sort.</typeparam>
    /// <param name="builder">The query builder to build the query from.</param>
    /// <param name="source">The query to apply the sorting to.</param>
    /// <returns>A new query with the sorting applied.</returns>
    public static IQueryable<T> Apply<T>(this ISortQueryBuilder<T> builder, IQueryable<T> source)
    {
        return builder.BuildAndReset().Apply(source);
    }
}