using System;
using System.Collections.Generic;

namespace FluentSortingDotNet;

/// <summary>
/// Represents a context for sorting that contains valid and invalid sort parameters.
/// </summary>
/// <typeparam name="T">The type of items to sort.</typeparam>
/// <param name="validParameters">The valid sort parameters.</param>
/// <param name="invalidParameters">The invalid sort parameters.</param>
public sealed class SortContext<T>(IReadOnlyList<SortParameter>? validParameters = null, IReadOnlyList<string>? invalidParameters = null)
{
    /// <summary>
    /// Represents an empty <see cref="SortContext{T}"/> with no valid or invalid parameters.
    /// </summary>
    public static readonly SortContext<T> Empty = new();

    /// <summary>
    /// Gets a value indicating whether the sort context is valid.
    /// </summary>
    public bool IsValid => InvalidParameters.Count == 0;

    /// <summary>
    /// Gets a value indicating whether the sort context is empty.
    /// </summary>
    public bool IsEmpty => ValidParameters.Count == 0;

    /// <summary>
    /// Gets the valid sort parameters.
    /// </summary>
    public IReadOnlyList<SortParameter> ValidParameters { get; } = validParameters ?? Array.Empty<SortParameter>();

    /// <summary>
    /// Gets the invalid sort parameters.
    /// </summary>
    public IReadOnlyList<string> InvalidParameters { get; } = invalidParameters ?? Array.Empty<string>();
}