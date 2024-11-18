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
    private readonly Dictionary<string, ISortableParameter<T>> _parameters;
    private readonly HashSet<ISortableParameter<T>> _defaultParameters = [];

    /// <summary>
    /// Creates a new instance of the <see cref="Sorter{T}"/> class.
    /// </summary>
    protected Sorter()
    {
        var builder = new SortBuilder<T>();
        Configure(builder);

        List<ISortableParameter<T>> parameters = builder.Build();

        _parameters = new(parameters.Count);

        foreach (ISortableParameter<T> parameter in parameters)
        {
#if NET8_0_OR_GREATER
            if (!_parameters.TryAdd(parameter.Name, parameter))
            {
                throw new InvalidOperationException($"The parameter name '{parameter.Name}' is already in use.");
            }
#else
            if (_parameters.ContainsKey(parameter.Name))
            {
                throw new InvalidOperationException($"The parameter name '{parameter.Name}' is already in use.");
            }

            _parameters[parameter.Name] = parameter;
#endif

            if (parameter.DefaultDirection.HasValue)
            {
                _defaultParameters.Add(parameter);
            }
        }
    }

    /// <summary>
    /// Configures the sorter with the sort parameters.
    /// </summary>
    /// <param name="builder">The builder to use to configure the sorter.</param>
    protected abstract void Configure(SortBuilder<T> builder);

    /// <summary>
    /// Sorts the specified query using the default sort parameters.
    /// </summary>
    /// <param name="query">The query to sort.</param>
    /// <returns>A <see cref="SortResult{T}"/> that represents the result of the sort operation.</returns>
    public SortResult<T> SortDefault(IQueryable<T> query)
        => Sort(query, []);

    private SortResult<T> GetAllInvalidParameters(SortParameter sourceParameter, IEnumerator<SortParameter> enumerator)
    {
        List<SortParameter> invalidSortParameters = [sourceParameter];

        while (enumerator.MoveNext())
        {
            SortParameter sortParameter = enumerator.Current;
            if (!_parameters.ContainsKey(sortParameter.Name))
            {
                invalidSortParameters.Add(sortParameter);
            }
        }

        return SortResult<T>.Invalid(invalidSortParameters);
    }

    /// <summary>
    /// Sorts the specified query using the specified sort parameters.
    /// </summary>
    /// <param name="query">The query to sort.</param>
    /// <param name="sortParameters">The sort parameters to use.</param>
    /// <returns>A <see cref="SortResult{T}"/> that represents the result of the sort operation.</returns>
    public SortResult<T> Sort(IQueryable<T> query, IEnumerable<SortParameter> sortParameters)
    {
        var first = true;
        IEnumerator<SortParameter> enumerator = sortParameters.GetEnumerator();

        while (enumerator.MoveNext())
        {
            SortParameter sortParameter = enumerator.Current;
            if (_parameters.TryGetValue(sortParameter.Name, out ISortableParameter<T>? sortableParameter))
            {
                query = SortParameter(query, first, sortableParameter.Expression, sortParameter.Direction);
                first = false;
            }
            else
            {
                return GetAllInvalidParameters(sortParameter, enumerator);
            }
        }

        if (first)
        {
            foreach (ISortableParameter<T> defaultParameter in _defaultParameters)
            {
                query = SortParameter(query, first, defaultParameter.Expression, defaultParameter.DefaultDirection!.Value);
                first = false;
            }
        }

        return SortResult<T>.Valid((IOrderedQueryable<T>)query);
    }

    private static IOrderedQueryable<T> SortParameter(IQueryable<T> query, bool first, LambdaExpression expression, SortDirection direction)
    {
        if (!first)
        {
            return direction switch
            {
                SortDirection.Ascending => ((IOrderedQueryable<T>)query).ThenBy(expression),
                SortDirection.Descending => ((IOrderedQueryable<T>)query).ThenByDescending(expression),
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
