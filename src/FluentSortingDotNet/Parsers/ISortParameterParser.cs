using System;

namespace FluentSortingDotNet.Parsers;

/// <summary>
/// Represents a parser that can parse a string into a collection of <see cref="SortParameter"/>.
/// </summary>
public interface ISortParameterParser
{
    /// <summary>
    /// Tries to get the next parameter from the query.
    /// </summary>
    /// <param name="query">The query to get the next parameter from.</param>
    /// <param name="parameter">The next parameter from the query.</param>
    /// <returns><see langword="true"/> if the next parameter was successfully retrieved; otherwise, <see langword="false"/>.</returns>
    bool TryGetNextParameter(ref ReadOnlySpan<char> query, out ReadOnlySpan<char> parameter);

    /// <summary>
    /// Tries to parse the specified parameter into a <see cref="SortParameter"/>.
    /// </summary>
    /// <param name="parameter">The parameter to parse.</param>
    /// <param name="sortParameter">The parsed <see cref="SortParameter"/>.</param>
    /// <returns><see langword="true"/> if the parameter was successfully parsed; otherwise, <see langword="false"/>.</returns>
    bool TryParseParameter(ReadOnlySpan<char> parameter, out SortParameter sortParameter);
}

