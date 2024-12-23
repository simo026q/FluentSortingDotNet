using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentSortingDotNet.Internal;

namespace FluentSortingDotNet.Queries;

/// <inheritdoc />
public sealed class DefaultSortQueryBuilder<T> : ISortQueryBuilder<T>, ISortQuery<T>
{
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

        return this;
    }

    /// <inheritdoc />
    public IQueryable<T> Apply(IQueryable<T> query)
    {
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