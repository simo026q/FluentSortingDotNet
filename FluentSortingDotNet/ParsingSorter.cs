using FluentSortingDotNet.Parser;
using System;
using System.Linq;

namespace FluentSortingDotNet;

/// <inheritdoc/>
/// <param name="sortParameterParser">The parser to use to parse the sort query.</param>
public abstract class ParsingSorter<T>(ISortParameterParser sortParameterParser) : Sorter<T>
{
    private readonly ISortParameterParser _sortParameterParser = sortParameterParser;

    /// <summary>
    /// Sorts the specified query using the specified sort query.
    /// </summary>
    /// <param name="query">The query to sort.</param>
    /// <param name="sortQuery">The sort query to use.</param>
    /// <returns>A <see cref="SortResult{T}"/> that represents the result of the sort operation.</returns>
    public SortResult<T> Sort(IQueryable<T> query, ReadOnlySpan<char> sortQuery) 
        => Sort(query, _sortParameterParser.Parse(sortQuery));
}
