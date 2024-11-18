using System;
using System.Collections.Generic;

namespace FluentSortingDotNet.Parser;

public class DefaultSortParameterParser : ISortParameterParser
{
    public IEnumerable<SortParameter> Parse(ReadOnlySpan<char> query)
    {
        List<SortParameter> sortParameters = new();

        while (TryGetNextPart(ref query, out ReadOnlySpan<char> part))
        {
            if (TryParsePart(part, out SortParameter sortParameter))
            {
                sortParameters.Add(sortParameter);
            }
        }

        return sortParameters;
    }

    protected virtual bool TryGetNextPart(ref ReadOnlySpan<char> query, out ReadOnlySpan<char> part)
    {
        if (query.IsEmpty)
        {
            part = ReadOnlySpan<char>.Empty;
            return false;
        }

        var index = query.IndexOf(',');
        if (index == -1)
        {
            part = query;
            query = ReadOnlySpan<char>.Empty;
            return true;
        }

        part = query.Slice(0, index);
        query = query.Slice(index + 1);
        return true;
    }

    protected virtual bool TryParsePart(ReadOnlySpan<char> part, out SortParameter sortParameter)
    {
        SortDirection direction = SortDirection.Ascending;

        if (part.IsEmpty)
        {
            sortParameter = default;
            return false;
        }

        if (part[0] == '-')
        {
            direction = SortDirection.Descending;
            part = part.Slice(1);
        }

        sortParameter = new SortParameter(part.ToString(), direction);
        return true;
    }
}
