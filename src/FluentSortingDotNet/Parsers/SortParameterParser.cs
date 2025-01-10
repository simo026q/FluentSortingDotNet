using System;

namespace FluentSortingDotNet.Parsers;

/// <inheritdoc />
public abstract class SortParameterParser : ISortParameterParser
{
    /// <inheritdoc/>
    public virtual bool TryGetNextParameter(ref ReadOnlySpan<char> query, out ReadOnlySpan<char> parameter)
    {
        if (query.IsEmpty)
        {
            parameter = ReadOnlySpan<char>.Empty;
            return false;
        }

        var index = IndexOfSeparator(query);
        if (index == -1)
        {
            parameter = query;
            query = ReadOnlySpan<char>.Empty;
            return true;
        }

        parameter = query.Slice(0, index);
        query = query.Slice(index + 1);
        return true;
    }

    /// <inheritdoc/>
    public abstract bool TryParseParameter(ReadOnlySpan<char> parameter, out SortParameter sortParameter);

    /// <summary>
    /// Returns the index of the separator in the query.
    /// </summary>
    /// <param name="query">The query to search for the separator.</param>
    /// <returns>The index of the separator in the query.</returns>
    protected abstract int IndexOfSeparator(ReadOnlySpan<char> query);
}
