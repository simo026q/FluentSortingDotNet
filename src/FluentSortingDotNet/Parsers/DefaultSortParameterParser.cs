using System;
using System.Runtime.CompilerServices;

namespace FluentSortingDotNet.Parsers;

/// <inheritdoc />
public sealed class DefaultSortParameterParser : SortParameterParser
{
    /// <summary>
    /// Gets the instance of the <see cref="DefaultSortParameterParser"/>.
    /// </summary>
    public static DefaultSortParameterParser Instance { get; } = new();

    private DefaultSortParameterParser()
    {
    }

    /// <inheritdoc />
    protected override int IndexOfSeparator(ReadOnlySpan<char> query) 
        => query.IndexOf(',');

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool TryParseParameter(ReadOnlySpan<char> parameter, out SortParameter sortParameter)
    {
        if (parameter.IsEmpty)
        {
            sortParameter = SortParameter.Empty;
            return false;
        }

        SortDirection direction = SortDirection.Ascending;

        if (parameter[0] == '-')
        {
            if (parameter.Length == 1)
            {
                sortParameter = SortParameter.Empty;
                return false;
            }

            direction = SortDirection.Descending;
            parameter = parameter.Slice(1);
        }

        var parameterName = parameter.ToString();
        sortParameter = new SortParameter(parameterName, direction);
        return true;
    }
}
