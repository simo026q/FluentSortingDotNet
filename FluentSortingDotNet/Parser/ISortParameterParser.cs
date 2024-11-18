using System;
using System.Collections.Generic;

namespace FluentSortingDotNet.Parser;

public interface ISortParameterParser
{
    IEnumerable<SortParameter> Parse(ReadOnlySpan<char> query);
}
