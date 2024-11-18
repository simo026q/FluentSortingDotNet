using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using FluentSortingDotNet.Internal;

namespace FluentSortingDotNet;

/// <summary>
/// Represents a sorter that can sort a collection of items.
/// </summary>
/// <typeparam name="T">The type of items to sort.</typeparam>
public abstract class Sorter<T>
{
    private static readonly List<SortParameter> EmptySortParameters = new(0);
    private readonly List<ISortableParameter<T>> _sortableParameters = new();

    /// <summary>
    /// Creates a new <see cref="SortParameterBuilder{T, TProperty}"/> for the specified property.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property to sort.</typeparam>
    /// <param name="expression">The lambda expression that represents the property to sort.</param>
    /// <returns>A new <see cref="SortParameterBuilder{T, TProperty}"/> instance.</returns>
    protected SortParameterBuilder<T, TProperty> ForParameter<TProperty>(Expression<Func<T, TProperty>> expression)
    {
        var parameter = new SortableParameter<T, TProperty>(expression);
        _sortableParameters.Add(parameter);
        return new SortParameterBuilder<T, TProperty>(parameter);
    }

    /// <summary>
    /// Sorts the specified query using the default sort parameters.
    /// </summary>
    /// <param name="query">The query to sort.</param>
    /// <returns>A <see cref="SortResult{T}"/> that represents the result of the sort operation.</returns>
    public SortResult<T> SortDefault(IQueryable<T> query)
        => Sort(query, EmptySortParameters);

    /// <summary>
    /// Sorts the specified query using the specified sort parameters.
    /// </summary>
    /// <param name="query">The query to sort.</param>
    /// <param name="sortParameters">The sort parameters to use.</param>
    /// <returns>A <see cref="SortResult{T}"/> that represents the result of the sort operation.</returns>
    public SortResult<T> Sort(IQueryable<T> query, IEnumerable<SortParameter> sortParameters)
    {
        List<SortParameter> invalidSortParameters = new();
        List<string> missingSortParameterNames = new();
        Dictionary<string, CombinedSortParameter<T>> combinedSortParameterMap = new();

        foreach (SortParameter sortParameter in sortParameters)
        {
            ISortableParameter<T>? sortableParameter = _sortableParameters.FirstOrDefault(x => x.Name == sortParameter.Name);
            if (sortableParameter == null)
            {
                invalidSortParameters.Add(sortParameter);
            }
            else
            {
                combinedSortParameterMap[sortParameter.Name] = new(sortableParameter, sortParameter);
            }
        }

        foreach (ISortableParameter<T> sortableParameter in _sortableParameters)
        {
            if (sortableParameter.IsRequired && !combinedSortParameterMap.ContainsKey(sortableParameter.Name))
            {
                missingSortParameterNames.Add(sortableParameter.Name);
            }
        }

        if (invalidSortParameters.Count > 0 || missingSortParameterNames.Count > 0)
        {
            return SortResult<T>.Invalid(invalidSortParameters, missingSortParameterNames);
        }

        if (combinedSortParameterMap.Count == 0)
        {
            // add default sort parameters
            foreach (ISortableParameter<T> defaultParameter in _sortableParameters.Where(x => x.DefaultDirection.HasValue))
            {
                combinedSortParameterMap[defaultParameter.Name] = new(defaultParameter, new(defaultParameter.Name, defaultParameter.DefaultDirection!.Value));
            }
        }

        var first = true;
        foreach (KeyValuePair<string, CombinedSortParameter<T>> entry in combinedSortParameterMap)
        {
            query = SortParameter(query, first, entry.Value.SortableParameter.Expression, entry.Value.SortParameter.Direction);
            first = false;
        }

        return SortResult<T>.Valid((IOrderedQueryable<T>)query);
    }

    private IOrderedQueryable<T> SortParameter(IQueryable<T> query, bool first, LambdaExpression expression, SortDirection direction)
    {
        if (!first && query is IOrderedQueryable<T> orderedQuery)
        {
            return direction switch
            {
                SortDirection.Ascending => orderedQuery.ThenBy(expression),
                SortDirection.Descending => orderedQuery.ThenByDescending(expression),
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }
        else
        {
            return direction switch
            {
                SortDirection.Ascending => query.OrderBy(expression),
                SortDirection.Descending => query.OrderByDescending(expression),
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }
    }
}
