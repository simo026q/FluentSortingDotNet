using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using FluentSortingDotNet.Internal;
using FluentSortingDotNet.Parser;

#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#endif

namespace FluentSortingDotNet;

/// <summary>
/// Represents a sorter that can sort a <see cref="IQueryable{T}"/> with a string based sort query. 
/// </summary>
/// <typeparam name="T">The type of items to sort.</typeparam>
public abstract class Sorter<T>
{
    private readonly ISortParameterParser _parser;
    private readonly IDictionary<string, SortableParameter> _parameters;
    private readonly List<SortableParameter> _defaultParameters;

    /// <summary>
    /// Creates a new instance of the <see cref="Sorter{T}"/> class.
    /// </summary>
    /// <param name="parser">The parser to use to parse the string based sort query.</param>
    protected Sorter(ISortParameterParser parser)
    {
        _parser = parser;

        var builder = new SortBuilder<T>();
        Configure(builder);

        List<SortableParameter> parameters = builder.Build();

        var parametersDictionary = new Dictionary<string, SortableParameter>(parameters.Count);
        _defaultParameters = new();

        foreach (SortableParameter parameter in parameters)
        {
#if NETCOREAPP2_0_OR_GREATER
            if (!parametersDictionary.TryAdd(parameter.Name, parameter))
            {
                throw ParameterAlreadyExists(parameter.Name);
            }
#else
            if (parametersDictionary.ContainsKey(parameter.Name))
            {
                throw ParameterAlreadyExists(parameter.Name);
            }

            parametersDictionary[parameter.Name] = parameter;
#endif

            if (parameter.DefaultDirection.HasValue)
            {
                _defaultParameters.Add(parameter);
            }
        }

#if NET8_0_OR_GREATER
        _parameters = parametersDictionary.ToFrozenDictionary();
#else
        _parameters = parametersDictionary;
#endif

        _defaultParameters.TrimExcess();

        static InvalidOperationException ParameterAlreadyExists(string name)
        {
            // This method will likely be inlined by the compiler
            return new($"A parameter with the name '{name}' already exists.");
        }
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Sorter{T}"/> class with the default sort parameter parser.
    /// </summary>
    protected Sorter() : this(DefaultSortParameterParser.Instance) { }

    /// <summary>
    /// Configures the sorter with the sort parameters.
    /// </summary>
    /// <param name="builder">The builder to use to configure the sorter.</param>
    protected abstract void Configure(SortBuilder<T> builder);

    /// <summary>
    /// Sorts the <paramref name="query"/> with the default sort parameters.
    /// </summary>
    /// <param name="query">The <see cref="IQueryable{T}"/> to sort.</param>
    /// <returns>A <see cref="SortResult"/> that represents the result of the sorting operation. When <see cref="SortResult.IsSuccess"/> is <see langword="true"/> the refence of <paramref name="query"/> is updated with the sorted <see cref="IQueryable{T}"/>.</returns>
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
    /// Sorts the <paramref name="query"/> with the specified <paramref name="sortQuery"/>. If <paramref name="sortQuery"/> is <see langword="null"/> or empty, the default sort parameters are used.
    /// </summary>
    /// <param name="query">The <see cref="IQueryable{T}"/> to sort.</param>
    /// <param name="sortQuery">The string based sort query to use to sort the query or <see langword="null"/> to use the default sort parameters.</param>
    /// <returns>A <see cref="SortResult"/> that represents the result of the sorting operation. When <see cref="SortResult.IsSuccess"/> is <see langword="true"/> the refence of <paramref name="query"/> is updated with the sorted <see cref="IQueryable{T}"/>.</returns>
    public SortResult Sort(ref IQueryable<T> query, string? sortQuery)
    {
        // when sortQuery is null, AsSpan() will return default(ReadOnlySpan<char>)
        return Sort(ref query, sortQuery.AsSpan());
    }

    /// <summary>
    /// Sorts the <paramref name="query"/> with the specified <paramref name="sortQuerySpan"/>. If <paramref name="sortQuerySpan"/> is empty, the default sort parameters are used.
    /// </summary>
    /// <param name="query">The <see cref="IQueryable{T}"/> to sort.</param>
    /// <param name="sortQuerySpan">The string based sort query to use to sort the query.</param>
    /// <returns>A <see cref="SortResult"/> that represents the result of the sorting operation. When <see cref="SortResult.IsSuccess"/> is <see langword="true"/> the refence of <paramref name="query"/> is updated with the sorted <see cref="IQueryable{T}"/>.</returns>
    public SortResult Sort(ref IQueryable<T> query, ReadOnlySpan<char> sortQuerySpan)
    {
        if (sortQuerySpan.IsEmpty)
        {
            return Sort(ref query);
        }

        var first = true;
        IQueryable<T> queryCopy = query;

        while (_parser.TryGetNextParameter(ref sortQuerySpan, out ReadOnlySpan<char> parameter))
        {
            if (_parser.TryParseParameter(parameter, out SortParameter sortParameter))
            {
                if (_parameters.TryGetValue(sortParameter.Name, out SortableParameter? sortableParameter))
                {
                    queryCopy = SortParameter(queryCopy, first, sortableParameter.Expression, sortParameter.Direction);
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

        query = queryCopy;

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

    // Consider adding abstraction for the concrete sorting implementation.
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
