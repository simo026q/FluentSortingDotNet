using System;

namespace FluentSortingDotNet.Parser;

/// <inheritdoc />
public sealed class DefaultSortParameterParser : SortParameterParser
{
    /// <inheritdoc />
    protected override int IndexOfSeparator(ReadOnlySpan<char> query) 
        => query.IndexOf(',');

    /// <inheritdoc />
    public override bool TryParseParameter(ReadOnlySpan<char> parameter, out SortParameter sortParameter)
    {
        SortDirection direction = SortDirection.Ascending;

        if (parameter.IsEmpty)
        {
            sortParameter = default;
            return false;
        }

        if (parameter[0] == '-')
        {
            direction = SortDirection.Descending;
            parameter = parameter.Slice(1);
        }

        var parameterName = parameter.ToString();
        sortParameter = new SortParameter(parameterName, direction);
        return true;
    }
}
