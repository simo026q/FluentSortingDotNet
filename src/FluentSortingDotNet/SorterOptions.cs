using System;
using System.Collections.Generic;

namespace FluentSortingDotNet;

/// <summary>
/// The options for a <see cref="Sorter{T}"/>.
/// </summary>
public sealed class SorterOptions
{
    /// <summary>
    /// Gets the default instance of the <see cref="SorterOptions"/> class.
    /// </summary>
    public static readonly SorterOptions Default = new();

    /// <summary>
    /// Gets or sets the comparer to use to compare the parameter names. The default is <see cref="StringComparer.Ordinal"/>.
    /// </summary>
    public IEqualityComparer<string> ParameterNameComparer { get; set; } = StringComparer.Ordinal;

    /// <summary>
    /// Gets or sets a value indicating whether to ignore invalid parameters. The default is <see langword="false"/>.
    /// </summary>
    public bool IgnoreInvalidParameters { get; set; } = false;
}