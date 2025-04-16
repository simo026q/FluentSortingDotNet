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
    /// Set the sort parameter as a default parameter when the sort query is empty.
    /// </summary>
    /// <param name="direction">The sort direction of the default parameter.</param>
    /// <returns>The current builder instance.</returns>
    public SortParameterBuilder IsDefault(SortDirection direction)
    {
        _parameter.DefaultDirection = direction;
        return this;
    }

    /// <summary>
    /// Set the name of the sort parameter. By default, the name is inferred from the property expression.
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <returns>The current builder instance.</returns>
    public SortParameterBuilder WithName(string name)
    {
        _parameter.Name = name;
        return this;
    }
}
