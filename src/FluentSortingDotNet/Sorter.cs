using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using FluentSortingDotNet.Internal;
using FluentSortingDotNet.Parser;

namespace FluentSortingDotNet;

/// <summary>
/// Represents a sorter that can sort a collection of items.
/// </summary>
/// <typeparam name="T">The type of items to sort.</typeparam>
public abstract class Sorter<T>
{
    private readonly ISortParameterParser _parser;
    private readonly Dictionary<string, SortableParameter> _parameters;
    private readonly List<SortableParameter> _defaultParameters;

    /// <summary>
    /// Creates a new instance of the <see cref="Sorter{T}"/> class.
    /// </summary>
    /// <param name="parser">The parser to use to parse the sort query.</param>
    protected Sorter(ISortParameterParser parser)
    {
        _parser = parser;

        var builder = new SortBuilder<T>();
        Configure(builder);

        List<SortableParameter> parameters = builder.Build();

        _parameters = new(parameters.Count);
        _defaultParameters = new();

        foreach (SortableParameter parameter in parameters)
        {
#if NETCOREAPP2_0_OR_GREATER
            if (!_parameters.TryAdd(parameter.Name, parameter))
            {
                throw ParameterAlreadyExists(parameter.Name);
            }
#else
            if (_parameters.ContainsKey(parameter.Name))
            {
                throw ParameterAlreadyExists(parameter.Name);
            }

            _parameters[parameter.Name] = parameter;
#endif

            if (parameter.DefaultDirection.HasValue)
            {
                _defaultParameters.Add(parameter);
            }
        }

        _defaultParameters.TrimExcess();

        static InvalidOperationException ParameterAlreadyExists(string name)
        {
            return new($"A parameter with the name '{name}' already exists.");
        }
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Sorter{T}"/> class.
    /// </summary>
    protected Sorter() : this(new DefaultSortParameterParser()) { }

    /// <summary>
    /// Configures the sorter with the sort parameters.
    /// </summary>
    /// <param name="builder">The builder to use to configure the sorter.</param>
    protected abstract void Configure(SortBuilder<T> builder);

    /// <summary>
    /// Sorts the <paramref name="query"/> with the default parameters.
    /// </summary>
    /// <param name="query">The query to sort.</param>
    /// <returns>A <see cref="SortResult"/> that represents the result of the sorting operation.</returns>
    public SortResult Sort(ref IQueryable<T> query)
    {
        var first = true;

        foreach (SortableParameter parameter in _defaultParameters)
        {
            query = SortParameter(query, first, parameter.Expression, parameter.DefaultDirection!.Value);
            first = false;
        }

        return SortResult.Success();
    }

    /// <summary>
    /// Sorts the <paramref name="query"/> with the specified <paramref name="sortQuery"/>.
    /// </summary>
    /// <param name="query">The query to sort.</param>
    /// <param name="sortQuery">The sort query to use to sort the query.</param>
    /// <returns>A <see cref="SortResult"/> that represents the result of the sorting operation.</returns>
    public SortResult Sort(ref IQueryable<T> query, string? sortQuery)
    {
        // when sortQuery is null, AsSpan() will return default(ReadOnlySpan<char>)
        return Sort(ref query, sortQuery.AsSpan());
    }

    /// <summary>
    /// Sorts the <paramref name="query"/> with the specified <paramref name="sortQuerySpan"/>.
    /// </summary>
    /// <param name="query">The query to sort.</param>
    /// <param name="sortQuerySpan">The sort query to use to sort the query.</param>
    /// <returns>A <see cref="SortResult"/> that represents the result of the sorting operation.</returns>
    public SortResult Sort(ref IQueryable<T> query, ReadOnlySpan<char> sortQuerySpan)
    {
        if (sortQuerySpan.IsEmpty)
        {
            return Sort(ref query);
        }

        var first = true;

        while (_parser.TryGetNextParameter(ref sortQuerySpan, out ReadOnlySpan<char> parameter))
        {
            if (_parser.TryParseParameter(parameter, out SortParameter sortParameter))
            {
                if (_parameters.TryGetValue(sortParameter.Name, out SortableParameter? sortableParameter))
                {
                    query = SortParameter(query, first, sortableParameter.Expression, sortParameter.Direction);
                    first = false;
                }
                else
                {
                    return SortResult.Failure(GetInvalidParameters(sortParameter.Name, sortQuerySpan));
                }
            }
            else
            {
                return SortResult.Failure(GetInvalidParameters(parameter.ToString(), sortQuerySpan));
            }
        }

        return first 
            ? Sort(ref query) 
            : SortResult.Success();
    }

    private List<string> GetInvalidParameters(string intialInvalidParameter, ReadOnlySpan<char> sortQuerySpan)
    {
        var invalidParameters = new List<string>() { intialInvalidParameter };

        while (_parser.TryGetNextParameter(ref sortQuerySpan, out ReadOnlySpan<char> parameter))
        {
            if (!_parser.TryParseParameter(parameter, out SortParameter sortParameter))
            {
                invalidParameters.Add(parameter.ToString());
            }
            else if (!_parameters.ContainsKey(sortParameter.Name))
            {
                invalidParameters.Add(sortParameter.Name);
            }
        }

        return invalidParameters;
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
