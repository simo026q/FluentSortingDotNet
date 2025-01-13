using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentSortingDotNet.Internal;

namespace FluentSortingDotNet.Queries;

/// <summary>
/// Represents a class that can build a sort query.
/// </summary>
/// <typeparam name="T">The type of items to sort.</typeparam>
public sealed class DefaultSortQueryBuilder<T> : ISortQueryBuilder<T>, ISortQuery<T>
{
    private bool _built;
    private readonly List<SortExpression> _sortExpressions = new();

    /// <inheritdoc />
    public bool IsEmpty => _sortExpressions.Count == 0;

    /// <inheritdoc />
    public ISortQueryBuilder<T> SortBy(LambdaExpression expression, SortDirection sortDirection)
    {
        _sortExpressions.Add(new SortExpression(expression, sortDirection));
        return this;
    }

    /// <inheritdoc />
    public ISortQuery<T> Build()
    {
        if (IsEmpty)
            throw new InvalidOperationException("No sorting expressions have been added.");

        _built = true;
        return this;
    }

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">Thrown when the query has not been built.</exception>
    public IQueryable<T> Apply(IQueryable<T> query)
    {
        if (!_built)
            throw new InvalidOperationException("Cannot apply sorting to an unbuilt query.");

        IOrderedQueryable<T>? orderedQuery = null;

        foreach (SortExpression expression in _sortExpressions)
        {
            if (orderedQuery == null)
            {
                orderedQuery = expression.SortDirection switch
                {
                    SortDirection.Ascending => query.OrderBy(expression.Expression),
                    SortDirection.Descending => query.OrderByDescending(expression.Expression),
                    _ => throw new ArgumentOutOfRangeException(nameof(expression.SortDirection))
                };
            }
            else
            {
                orderedQuery = expression.SortDirection switch
                {
                    SortDirection.Ascending => orderedQuery.ThenBy(expression.Expression),
                    SortDirection.Descending => orderedQuery.ThenByDescending(expression.Expression),
                    _ => throw new ArgumentOutOfRangeException(nameof(expression.SortDirection))
                };
            }
        }

        return orderedQuery ?? query;
    }

    private sealed class SortExpression(LambdaExpression expression, SortDirection sortDirection)
    {
        public LambdaExpression Expression { get; } = expression;
        public SortDirection SortDirection { get; } = sortDirection;
    }
}