using System;
using System.Collections.Generic;
using System.Diagnostics;
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
public abstract class Sorter<T> : ISorter<T>
{
    private readonly ISortParameterParser _parser;
    private readonly ISortQueryBuilderFactory<T> _queryBuilderFactory;
    private readonly ISortQuery<T> _defaultQuery;

    private readonly SorterOptions _options;
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

        _options = builder.Options;

        List<SortableParameter> parameters = builder.Build();

        var parametersDictionary = new Dictionary<string, SortableParameter>(parameters.Count, _options.ParameterNameComparer);

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
    [Obsolete("This method is obsolete and will be removed in a future version. Use CreateSortQuery(SortContext<T>) or extensions methods instead.", error: false)]
    public SortResult Sort(ref IQueryable<T> query) 
        => Sort(ref query, ReadOnlySpan<char>.Empty);

    /// <summary>
    /// Sorts the <paramref name="query"/> with the specified <paramref name="sortQuery"/>. If <paramref name="sortQuery"/> is <see langword="null"/> or empty, the default sort parameters are used.
    /// </summary>
    /// <param name="query">The <see cref="IQueryable{T}"/> to sort.</param>
    /// <param name="sortQuery">The string based sort query to use to sort the query or <see langword="null"/> to use the default sort parameters.</param>
    /// <returns>A <see cref="SortResult"/> that represents the result of the sorting operation. When <see cref="SortResult.IsSuccess"/> is <see langword="true"/> the refence of <paramref name="query"/> is updated with the sorted <see cref="IQueryable{T}"/>.</returns>
    [Obsolete("This method is obsolete and will be removed in a future version. Use CreateSortQuery(SortContext<T>) or extensions methods instead.", error: false)]
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
    [Obsolete("This method is obsolete and will be removed in a future version. Use CreateSortQuery(SortContext<T>) or extensions methods instead.", error: false)]
    public SortResult Sort(ref IQueryable<T> query, ReadOnlySpan<char> sortQuerySpan)
    {
        var context = Validate(sortQuerySpan);
        if (!_options.IgnoreInvalidParameters && !context.IsValid)
        {
            return SortResult.Failure(context.InvalidParameters);
        }

        query = CreateSortQuery(context).Apply(query);
        return SortResult.Success();
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentException">The <paramref name="sortContext"/> contains invalid parameters and <see cref="SorterOptions.IgnoreInvalidParameters"/> is set to <see langword="false"/>.</exception>
    public ISortQuery<T> CreateSortQuery(SortContext<T> sortContext)
    {
        if (!_options.IgnoreInvalidParameters && !sortContext.IsValid)
        {
            throw new ArgumentException("The sort context contains invalid parameters.", nameof(sortContext));
        }

        if (sortContext.IsEmpty)
        {
            return _defaultQuery;
        }

        ISortQueryBuilder<T> queryBuilder = _queryBuilderFactory.Create();

        foreach (SortParameter sortParameter in sortContext.ValidParameters)
        {
            if (!_parameters.TryGetValue(sortParameter.Name, out SortableParameter? sortableParameter))
            {
                Debug.Fail("This should never happen. The parameter should have been validated before.");
                throw new InvalidOperationException($"The parameter '{sortParameter.Name}' is not valid.");
            }

            queryBuilder.SortBy(sortableParameter.Expression, sortParameter.Direction);
        }

        return queryBuilder.IsEmpty 
            ? _defaultQuery 
            : queryBuilder.Build();
    }

    /// <inheritdoc/>
    public SortContext<T> Validate(ReadOnlySpan<char> sortQuery)
    {
        List<SortParameter> validSortParameters = new();
        List<string>? invalidSortParameters = null;

        while (_parser.TryGetNextParameter(ref sortQuery, out ReadOnlySpan<char> parameter))
        {
            if (!_parser.TryParseParameter(parameter, out SortParameter sortParameter))
            {
                invalidSortParameters ??= new();
                invalidSortParameters.Add(parameter.ToString());
            }
            else if (!_parameters.ContainsKey(sortParameter.Name))
            {
                invalidSortParameters ??= new();
                invalidSortParameters.Add(sortParameter.Name);
            }
            else
            {
                validSortParameters.Add(sortParameter);
            }
        }

        return new SortContext<T>(validSortParameters, invalidSortParameters);
    }
}
