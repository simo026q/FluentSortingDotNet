﻿using System;
using System.Collections.Generic;

namespace FluentSortingDotNet.Parser;

/// <inheritdoc />
public abstract class SortParameterParser : ISortParameterParser
{
    /// <inheritdoc />
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

    /// <summary>
    /// Tries to get the next part of the query.
    /// </summary>
    /// <param name="query">The query to get the next part from.</param>
    /// <param name="part">The next part of the query.</param>
    /// <returns><see langword="true"/> if the next part was successfully retrieved; otherwise, <see langword="false"/>.</returns>
    protected virtual bool TryGetNextPart(ref ReadOnlySpan<char> query, out ReadOnlySpan<char> part)
    {
        if (query.IsEmpty)
        {
            part = ReadOnlySpan<char>.Empty;
            return false;
        }

        var index = IndexOfSeparator(query);
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

    /// <summary>
    /// Returns the index of the separator in the query.
    /// </summary>
    /// <param name="query">The query to search for the separator.</param>
    /// <returns>The index of the separator in the query.</returns>
    protected abstract int IndexOfSeparator(ReadOnlySpan<char> query);

    /// <summary>
    /// Tries to parse the specified part into a <see cref="SortParameter"/>.
    /// </summary>
    /// <param name="part">The part to parse.</param>
    /// <param name="sortParameter">The parsed <see cref="SortParameter"/>.</param>
    /// <returns><see langword="true"/> if the part was successfully parsed; otherwise, <see langword="false"/>.</returns>
    protected abstract bool TryParsePart(ReadOnlySpan<char> part, out SortParameter sortParameter);
}
