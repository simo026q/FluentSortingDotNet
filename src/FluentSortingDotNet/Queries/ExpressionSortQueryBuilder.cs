using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FluentSortingDotNet.Internal;

namespace FluentSortingDotNet.Queries;

internal sealed class OrderMethodCache<T>(MethodInfo genericMethod)
{
    private readonly MethodInfo _genericMethod = genericMethod;
    private readonly Dictionary<Type, MethodInfo> _methods = new();

    public MethodInfo GetMethod(Type returnType)
    {
        if (_methods.TryGetValue(returnType, out MethodInfo? method))
            return method;

        method = _genericMethod.MakeGenericMethod(typeof(T), returnType);

        _methods.Add(returnType, method);
        return method;
    }
}

public sealed class ExpressionSortQueryBuilder<T> : ISortQueryBuilder<T>
{
    private static readonly Type Type = typeof(T);
    private static readonly ParameterExpression QueryParameter = Expression.Parameter(typeof(IQueryable<T>), "query");
    private static readonly ParameterExpression LambdaParameter = Expression.Parameter(Type, "x");

    private static readonly OrderMethodCache<T> OrderByCache = new(QueryableMethods.OrderBy);
    private static readonly OrderMethodCache<T> OrderByDescendingCache = new(QueryableMethods.OrderByDescending);
    private static readonly OrderMethodCache<T> ThenByCache = new(QueryableMethods.ThenBy);
    private static readonly OrderMethodCache<T> ThenByDescendingCache = new(QueryableMethods.ThenByDescending);

    private Expression? _sortExpression;

    public bool IsEmpty => _sortExpression == null;

    public void Reset()
    {
        _sortExpression = null;
    }

    public void SortBy(LambdaExpression expression, SortDirection direction)
    {
        LambdaExpression updatedLambda = ReplaceParameter(expression, expression.Parameters[0], LambdaParameter);

        if (_sortExpression == null)
        {
            MethodInfo method = direction == SortDirection.Ascending
                ? OrderByCache.GetMethod(updatedLambda.ReturnType)
                : OrderByDescendingCache.GetMethod(updatedLambda.ReturnType);

            _sortExpression = QueryableHelper.CreateMethodCall(method, QueryParameter, updatedLambda);
        }
        else
        {
            MethodInfo method = direction == SortDirection.Ascending
                ? ThenByCache.GetMethod(updatedLambda.ReturnType)
                : ThenByDescendingCache.GetMethod(updatedLambda.ReturnType);

            _sortExpression = QueryableHelper.CreateMethodCall(method, _sortExpression, updatedLambda);
        }
    }

    public ISortQuery<T> Build()
    {
        if (_sortExpression == null)
            throw new InvalidOperationException("No sorting expressions have been added.");

        return new LambdaSortQuery<T>(Expression.Lambda<Func<IQueryable<T>, IOrderedQueryable<T>>>(_sortExpression, QueryParameter).Compile());
    }

    private static LambdaExpression ReplaceParameter(LambdaExpression original, ParameterExpression toReplace, ParameterExpression replacement)
    {
        ParameterReplacer replacer = new(toReplace, replacement);
        Expression updatedBody = replacer.Visit(original.Body);
        return Expression.Lambda(updatedBody, replacement);
    }

    private sealed class ParameterReplacer(ParameterExpression toReplace, ParameterExpression replacement) : ExpressionVisitor
    {
        private readonly ParameterExpression _toReplace = toReplace;
        private readonly ParameterExpression _replacement = replacement;

        protected override Expression VisitParameter(ParameterExpression node) 
            => node == _toReplace ? _replacement : base.VisitParameter(node);
    }
}