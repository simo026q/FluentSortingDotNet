using FluentSortingDotNet.Internal;
using System;

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
    /// <param name="sortDirection">The sort direction of the default parameter.</param>
    /// <returns>The current builder instance.</returns>
    [Obsolete("Use IsDefault instead.", error: false)]
    public SortParameterBuilder Default(SortDirection sortDirection)
    {
        _parameter.DefaultDirection = sortDirection;
        return this;
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
    /// Specifies a custom name of the parameter.
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <returns>The current builder instance.</returns>
    [Obsolete("Use WithName instead.", error: false)]
    public SortParameterBuilder Name(string name)
    {
        _parameter.Name = name;
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
