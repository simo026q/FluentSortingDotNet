using System;

namespace FluentSortingDotNet.Parser;

/// <inheritdoc />
public sealed class DefaultSortParameterParser : SortParameterParser
{
    /// <inheritdoc />
    protected override int IndexOfSeparator(ReadOnlySpan<char> query) 
        => query.IndexOf(',');

    /// <inheritdoc />
    protected override bool TryParsePart(ReadOnlySpan<char> part, out SortParameter sortParameter)
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
