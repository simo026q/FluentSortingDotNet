using System;
using System.Collections.Generic;

namespace FluentSortingDotNet.Parser;

/// <summary>
/// Represents a parser that can parse a string into a collection of <see cref="SortParameter"/>.
/// </summary>
public interface ISortParameterParser
{
    /// <summary>
    /// Parses the specified string into a collection of <see cref="SortParameter"/>.
    /// </summary>
    /// <param name="query">The string to parse.</param>
    /// <returns>A collection of <see cref="SortParameter"/>.</returns>
    IEnumerable<SortParameter> Parse(ReadOnlySpan<char> query);
}
