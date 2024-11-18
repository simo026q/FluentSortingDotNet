using System;
using System.Collections.Generic;
using System.Linq;

namespace FluentSortingDotNet;

/// <summary>
/// Represents the result of a sort operation.
/// </summary>
/// <typeparam name="T">The type of the elements in the query.</typeparam>
public sealed class SortResult<T>
{
    private static readonly List<SortParameter> EmptyInvalidSortParameters = new(0);

    private readonly IOrderedQueryable<T>? _query;

    /// <summary>
    /// Gets the query that was sorted.
    /// </summary>
    /// <exception cref="InvalidOperationException">The query is invalid.</exception>
    public IOrderedQueryable<T> Query => _query ?? throw new InvalidOperationException("The query is invalid.");

    /// <summary>
    /// Invalid sort parameters that were used to sort the query.
    /// </summary>
    public List<SortParameter> InvalidSortParameters { get; }

    /// <summary>
    /// Indicates whether the query is valid.
    /// </summary>
    public bool IsValid => _query != null;

    private SortResult(IOrderedQueryable<T>? query, List<SortParameter>? invalidSortParameters)
    {
        _query = query;
        InvalidSortParameters = invalidSortParameters ?? EmptyInvalidSortParameters;
    }

    /// <summary>
    /// Gets the query that was sorted or <see langword="null"/> if the query is invalid.
    /// </summary>
    /// <returns>The query that was sorted or <see langword="null"/> if the query is invalid.</returns>
    public IOrderedQueryable<T>? GetQueryOrDefault()
        => _query;

    internal static SortResult<T> Invalid(List<SortParameter> invalidSortParameters)
        => new(null, invalidSortParameters);

    internal static SortResult<T> Valid(IOrderedQueryable<T> query)
        => new(query, null);
}
