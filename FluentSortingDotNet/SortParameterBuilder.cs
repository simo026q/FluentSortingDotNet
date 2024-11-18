using FluentSortingDotNet.Internal;

namespace FluentSortingDotNet;

/// <summary>
/// Used to build a sortable parameter.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TProperty"></typeparam>
public sealed class SortParameterBuilder<T, TProperty>
{
    private readonly SortableParameter<T, TProperty> _parameter;

    internal SortParameterBuilder(SortableParameter<T, TProperty> parameter)
    {
        _parameter = parameter;
    }

    /// <summary>
    /// Specifies a default sort parameter when none is provided.
    /// </summary>
    /// <param name="sortDirection">The sort direction of the default parameter.</param>
    /// <returns>The current builder instance.</returns>
    public SortParameterBuilder<T, TProperty> Default(SortDirection sortDirection)
    {
        _parameter.DefaultDirection = sortDirection;
        return this;
    }

    /// <summary>
    /// Marks the parameter as required.
    /// </summary>
    /// <returns>The current builder instance.</returns>
    public SortParameterBuilder<T, TProperty> Required()
    {
        _parameter.IsRequired = true;
        return this;
    }

    /// <summary>
    /// Specifies a custom name of the parameter.
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <returns>The current builder instance.</returns>
    public SortParameterBuilder<T, TProperty> Name(string name)
    {
        _parameter.Name = name;
        return this;
    }
}
