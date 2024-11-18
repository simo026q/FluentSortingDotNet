using System;
using System.Collections.Generic;
using System.Linq;

namespace FluentSortingDotNet;

public sealed class SortResult<T>
{
    private static readonly List<SortParameter> EmptyInvalidSortParameters = new(0);
    private static readonly List<string> EmptyMissingSortParameterNames = new(0);

    private readonly IOrderedQueryable<T>? _query;

    public IOrderedQueryable<T> Query => _query ?? throw new InvalidOperationException("The query is invalid.");
    public List<SortParameter> InvalidSortParameters { get; }
    public List<string> MissingSortParameterNames { get; }
    public bool IsValid => _query != null;

    private SortResult(IOrderedQueryable<T>? query, List<SortParameter> invalidSortParameters, List<string> missingSortParameterNames)
    {
        _query = query;
        InvalidSortParameters = invalidSortParameters;
        MissingSortParameterNames = missingSortParameterNames;
    }

    public IOrderedQueryable<T>? GetQueryOrDefault()
        => _query;

    public static SortResult<T> Invalid(List<SortParameter> invalidSortParameters, List<string> missingSortParameterNames)
        => new(null, invalidSortParameters, missingSortParameterNames);

    public static SortResult<T> Valid(IOrderedQueryable<T> query)
        => new(query, EmptyInvalidSortParameters, EmptyMissingSortParameterNames);
}
