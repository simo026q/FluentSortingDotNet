using FluentSortingDotNet.Queries;
using System;
using System.Linq;

namespace FluentSortingDotNet;

/// <summary>
/// Represents a sorter can create a sort query for a <see cref="IQueryable{T}"/>.
/// </summary>
/// <typeparam name="T">The type of items to sort.</typeparam>
public interface ISorter<T>
{
    /// <summary>
    /// Create a new instance of the <see cref="ISortQuery{T}"/> class with the specified <paramref name="sortContext"/>.
    /// </summary>
    /// <param name="sortContext">The <see cref="SortContext{T}"/> that contains the sort parameters.</param>
    /// <returns>A new instance of the <see cref="ISortQuery{T}"/> class.</returns>
    ISortQuery<T> CreateSortQuery(SortContext<T> sortContext);

    /// <summary>
    /// Validates the specified sort query and returns a <see cref="SortContext{T}"/> that can be used to sort a query.
    /// </summary>
    /// <param name="sortQuery">The sort query to validate.</param>
    /// <returns>A new <see cref="SortContext{T}"/> that contains the valid and invalid parameters.</returns>
    SortContext<T> Validate(ReadOnlySpan<char> sortQuery);
}