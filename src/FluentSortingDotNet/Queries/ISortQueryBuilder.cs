using System;
using System.Linq.Expressions;

namespace FluentSortingDotNet.Queries;

/// <summary>
/// Represents a query builder for sorting.
/// </summary>
/// <typeparam name="T">The type of items to sort.</typeparam>
public interface ISortQueryBuilder<T>
{
    /// <summary>
    /// Gets a value indicating whether the query builder has any sorting parameters.
    /// </summary>
    bool IsEmpty { get; }

    /// <summary>
    /// Sorts the query by the specified expression and sort direction.
    /// </summary>
    /// <param name="expression">The expression to sort by.</param>
    /// <param name="sortDirection">The direction to sort the expression by.</param>
    /// <returns>The current instance of the <see cref="ISortQueryBuilder{T}"/>.</returns>
    ISortQueryBuilder<T> SortBy(LambdaExpression expression, SortDirection sortDirection);

    /// <summary>
    /// Builds the sort query.
    /// </summary>
    /// <returns>Returns the built sort query.</returns>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="IsEmpty"/> is <see langword="true"/>.</exception>
    ISortQuery<T> Build();
}