using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentSortingDotNet.Internal;

namespace FluentSortingDotNet.Queries;

/// <inheritdoc />
public sealed class DefaultSortQueryBuilder<T> : ISortQueryBuilder<T>
{
    /// <summary>
    /// Gets the default instance of the <see cref="DefaultSortQueryBuilder{T}"/> class.
    /// </summary>
    public static readonly DefaultSortQueryBuilder<T> Instance = new();

    private List<SortExpression> _sortExpressions = new();

    /// <inheritdoc />
    public bool IsEmpty => _sortExpressions.Count == 0;

    private DefaultSortQueryBuilder()
    {
    }

    /// <inheritdoc />
    public void Reset() => _sortExpressions = new();

    /// <inheritdoc />
    public ISortQueryBuilder<T> SortBy(LambdaExpression expression, SortDirection sortDirection)
    {
        _sortExpressions.Add(new SortExpression(expression, sortDirection));
        return this;
    }

    /// <inheritdoc />
    public ISortQuery<T> Build()
    {
        if (_sortExpressions.Count == 0)
            throw new InvalidOperationException("No sorting expressions have been added.");

        return new LazySortQuery(_sortExpressions);
    }

    private sealed class SortExpression(LambdaExpression expression, SortDirection sortDirection)
    {
        public LambdaExpression Expression { get; } = expression;
        public SortDirection SortDirection { get; } = sortDirection;
    }

    private sealed class LazySortQuery(List<SortExpression> expressions) : ISortQuery<T>
    {
        private readonly List<SortExpression> _expressions = expressions;

        public IQueryable<T> Apply(IQueryable<T> query)
        {
            IOrderedQueryable<T>? orderedQuery = null;

            foreach (SortExpression expression in _expressions)
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
    }
}