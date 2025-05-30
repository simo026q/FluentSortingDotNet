using FluentSortingDotNet.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FluentSortingDotNet.Queries;

/// <summary>
/// Represents a class that can build a sort query.
/// </summary>
/// <typeparam name="T">The type of items to sort.</typeparam>
public sealed class DefaultSortQueryBuilder<T> : ISortQueryBuilder<T>
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
        return IsEmpty
            ? throw new InvalidOperationException("No sorting expressions have been added.")
            : new SortQuery(_sortExpressions);
    }

    private sealed class SortExpression(LambdaExpression expression, SortDirection sortDirection)
    {
        public LambdaExpression Expression { get; } = expression;
        public SortDirection SortDirection { get; } = sortDirection;
    }

    private sealed class SortQuery(List<SortExpression> sortExpressions) : ISortQuery<T>
    {
        private readonly List<SortExpression> _sortExpressions = sortExpressions;

        public IQueryable<T> Apply(IQueryable<T> query)
        {
            IOrderedQueryable<T>? orderedQuery = null;

            foreach (SortExpression expression in _sortExpressions)
            {
                if (orderedQuery == null)
                {
                    orderedQuery = expression.SortDirection == SortDirection.Ascending
                        ? query.OrderBy(expression.Expression)
                        : query.OrderByDescending(expression.Expression);
                }
                else
                {
                    orderedQuery = expression.SortDirection == SortDirection.Ascending
                        ? orderedQuery.ThenBy(expression.Expression)
                        : orderedQuery.ThenByDescending(expression.Expression);
                }
            }

            return orderedQuery ?? query;
        }
    }
}