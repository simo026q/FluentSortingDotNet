using FluentSortingDotNet.Parser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FluentSortingDotNet;

public abstract class ParsingSorter<T>(ISortParameterParser sortParameterParser) : Sorter<T>
{
    private readonly ISortParameterParser _sortParameterParser = sortParameterParser;

    public SortResult<T> Sort(IQueryable<T> query, ReadOnlySpan<char> sortQuery) 
        => Sort(query, _sortParameterParser.Parse(sortQuery));
}
