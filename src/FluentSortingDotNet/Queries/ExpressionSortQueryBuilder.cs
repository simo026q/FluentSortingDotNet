using FluentSortingDotNet.Internal;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FluentSortingDotNet.Queries;

/// <inheritdoc />
public sealed class ExpressionSortQueryBuilder<T> : ISortQueryBuilder<T>
{
    private static readonly Type Type = typeof(T);
    private static readonly ParameterExpression QueryParameter = Expression.Parameter(typeof(IQueryable<T>), "query");
    private static readonly ParameterExpression LambdaParameter = Expression.Parameter(Type, "x");

    private Expression? _sortExpression;

    /// <inheritdoc />
    public bool IsEmpty => _sortExpression == null;

    /// <inheritdoc />
    public ISortQueryBuilder<T> SortBy(LambdaExpression expression, SortDirection direction)
    {
        LambdaExpression updatedLambda = ReplaceParameter(expression, expression.Parameters[0], LambdaParameter);

        _sortExpression = _sortExpression == null
            ? CreateMethodCallExpression(QueryableMethods.GetOrderByMethod, QueryParameter)
            : CreateMethodCallExpression(QueryableMethods.GetThenByMethod, _sortExpression);

        return this;

        Expression CreateMethodCallExpression(Func<SortDirection, MethodInfo> methodProvider, Expression query)
        {
            return QueryableHelper.CreateMethodCall(methodProvider(direction).MakeGenericMethod(Type, updatedLambda.ReturnType), query, updatedLambda);
        }
    }

    /// <inheritdoc />
    public ISortQuery<T> Build()
    {
        if (_sortExpression == null)
        {
            throw new InvalidOperationException("No sorting expressions have been added.");
        }

        var lambda = Expression.Lambda<Func<IQueryable<T>, IQueryable<T>>>(_sortExpression, QueryParameter).Compile();
        return new DelegateSortQuery(lambda);
    }

    private static LambdaExpression ReplaceParameter(LambdaExpression original, ParameterExpression toReplace, ParameterExpression replacement)
    {
        ParameterReplacer replacer = new(toReplace, replacement);
        Expression updatedBody = replacer.Visit(original.Body);
        return Expression.Lambda(updatedBody, replacement);
    }

    private sealed class DelegateSortQuery(Func<IQueryable<T>, IQueryable<T>> sortDelegate) : ISortQuery<T>
    {
        private readonly Func<IQueryable<T>, IQueryable<T>> _sortDelegate = sortDelegate;

        public IQueryable<T> Apply(IQueryable<T> query) => _sortDelegate(query);
    }

    private sealed class ParameterReplacer(ParameterExpression toReplace, ParameterExpression replacement) : ExpressionVisitor
    {
        private readonly ParameterExpression _toReplace = toReplace;
        private readonly ParameterExpression _replacement = replacement;

        protected override Expression VisitParameter(ParameterExpression node)
            => node == _toReplace ? _replacement : base.VisitParameter(node);
    }
}