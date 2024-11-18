using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using FluentSortingDotNet.Internal;

namespace FluentSortingDotNet;

public abstract class Sorter<T>
{
    private static readonly List<SortParameter> EmptySortParameters = new(0);
    private readonly List<ISortableParameter<T>> _sortableParameters = new();

    protected SortParameterBuilder<T, TProperty> SortFor<TProperty>(Expression<Func<T, TProperty>> expression)
    {
        var parameter = new SortableParameter<T, TProperty>(expression);
        _sortableParameters.Add(parameter);
        return new SortParameterBuilder<T, TProperty>(parameter);
    }

    public SortResult<T> SortDefault(IQueryable<T> query)
        => Sort(query, EmptySortParameters);

    public virtual SortResult<T> Sort(IQueryable<T> query, IEnumerable<SortParameter> sortParameters)
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

    protected virtual IOrderedQueryable<T> SortParameter(IQueryable<T> query, bool first, LambdaExpression expression, SortDirection direction)
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
