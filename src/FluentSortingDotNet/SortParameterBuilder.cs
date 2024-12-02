using FluentSortingDotNet.Internal;

namespace FluentSortingDotNet;

/// <summary>
/// Used to build a sortable parameter.
/// </summary>
public sealed class SortParameterBuilder
{
    private readonly SortableParameter _parameter;

    internal SortParameterBuilder(SortableParameter parameter)
    {
        _parameter = parameter;
    }

    /// <summary>
    /// Specifies a default sort parameter when none is provided.
    /// </summary>
    /// <param name="sortDirection">The sort direction of the default parameter.</param>
    /// <returns>The current builder instance.</returns>
    public SortParameterBuilder Default(SortDirection sortDirection)
    {
        _parameter.DefaultDirection = sortDirection;
        return this;
    }

    /// <summary>
    /// Specifies a custom name of the parameter.
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <returns>The current builder instance.</returns>
    public SortParameterBuilder Name(string name)
    {
        _parameter.Name = name;
        return this;
    }
}
