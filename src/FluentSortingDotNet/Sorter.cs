using System;
using System.Collections.Generic;
using System.Linq;
using FluentSortingDotNet.Internal;
using FluentSortingDotNet.Parsers;
using FluentSortingDotNet.Queries;

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
    private readonly ISortQueryBuilderFactory<T> _queryBuilderFactory;
    private readonly ISortQuery<T> _defaultQuery;

    private readonly IDictionary<string, SortableParameter> _parameters;

    /// <summary>
    /// Creates a new instance of the <see cref="Sorter{T}"/> class.
    /// </summary>
    /// <param name="parser">The parser to use to parse the string based sort query.</param>
    /// <param name="sortQueryBuilderFactory">The factory used to create a query builder for applying the sort parameters.</param>
    /// <param name="defaultParameterSortQueryBuilder">The query builder used for applying the default sort parameters.</param>
    /// <param name="options">The options to use for the sorter.</param>
    protected Sorter(ISortParameterParser parser, ISortQueryBuilderFactory<T> sortQueryBuilderFactory, ISortQueryBuilder<T> defaultParameterSortQueryBuilder, SorterOptions? options = null)
    {
        _parser = parser;
        _queryBuilderFactory = sortQueryBuilderFactory;

        var builder = new SortBuilder<T>(options);
        Configure(builder);

        options = builder.Options;

        List<SortableParameter> parameters = builder.Build();

        var parametersDictionary = new Dictionary<string, SortableParameter>(parameters.Count, options.ParameterNameComparer);

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
                defaultParameterSortQueryBuilder.SortBy(parameter.Expression, parameter.DefaultDirection.Value);
            }
        }

#if NET8_0_OR_GREATER
        _parameters = parametersDictionary.ToFrozenDictionary(_options.ParameterNameComparer);
#else
        _parameters = parametersDictionary;
#endif

        _defaultQuery = defaultParameterSortQueryBuilder.IsEmpty 
            ? NullSortQuery<T>.Instance 
            : defaultParameterSortQueryBuilder.Build();

        static InvalidOperationException ParameterAlreadyExists(string name)
        {
            // This method will likely be inlined by the compiler
            return new($"A parameter with the name '{name}' already exists.");
        }
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Sorter{T}"/> class.
    /// </summary>
    /// <param name="parser">The parser to use to parse the string based sort query.</param>
    /// <param name="sortQueryBuilderFactory">The factory used to create a query builder for applying the sort parameters.</param>
    /// <param name="options">The options to use for the sorter.</param>
    protected Sorter(ISortParameterParser parser, ISortQueryBuilderFactory<T> sortQueryBuilderFactory, SorterOptions? options = null) : this(parser, sortQueryBuilderFactory, new ExpressionSortQueryBuilder<T>(), options) { }

    /// <summary>
    /// Creates a new instance of the <see cref="Sorter{T}"/> class with the default sort query builder.
    /// </summary>
    /// <param name="parser">The parser to use to parse the string based sort query.</param>
    /// <param name="options">The options to use for the sorter.</param>
    protected Sorter(ISortParameterParser parser, SorterOptions? options = null) : this(parser, DefaultSortQueryBuilderFactory<T>.Instance, options) { }

    /// <summary>
    /// Creates a new instance of the <see cref="Sorter{T}"/> class with the default sort parameter parser.
    /// </summary>
    /// <param name="options">The options to use for the sorter.</param>
    protected Sorter(SorterOptions? options = null) : this(DefaultSortParameterParser.Instance, options) { }

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
        query = _defaultQuery.Apply(query);
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

        ISortQueryBuilder<T> queryBuilder = _queryBuilderFactory.Create();

        while (_parser.TryGetNextParameter(ref sortQuerySpan, out ReadOnlySpan<char> parameter))
        {
            if (!_parser.TryParseParameter(parameter, out SortParameter sortParameter))
            {
                return SortResult.Failure(GetInvalidParameters(parameter.ToString(), sortQuerySpan));
            }

            if (!_parameters.TryGetValue(sortParameter.Name, out SortableParameter? sortableParameter))
            {
                return SortResult.Failure(GetInvalidParameters(sortParameter.Name, sortQuerySpan));
            }

            queryBuilder.SortBy(sortableParameter.Expression, sortParameter.Direction);
        }

        if (queryBuilder.IsEmpty)
        {
            return Sort(ref query);
        }

        query = queryBuilder.Build().Apply(query);
        return SortResult.Success();
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
}
