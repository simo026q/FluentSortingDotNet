namespace FluentSortingDotNet;

/// <summary>
/// Represents a unvalidated parameter used to sort a collection.
/// </summary>
/// <param name="name">The name of the parameter to sort by.</param>
/// <param name="direction">The direction to sort the parameter by.</param>
public readonly struct SortParameter(string name, SortDirection direction)
{
    /// <summary>
    /// Represents an empty <see cref="SortParameter"/>.
    /// </summary>
    public static readonly SortParameter Empty = new(string.Empty, SortDirection.Ascending);

    /// <summary>
    /// Gets the name of the parameter to sort by.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Gets the direction to sort the parameter by.
    /// </summary>
    public SortDirection Direction { get; } = direction;
}
