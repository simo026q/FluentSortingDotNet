using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentSortingDotNet.Internal;

namespace FluentSortingDotNet;

/// <summary>
/// Represents a builder that can be used to configure a sorter.
/// </summary>
/// <typeparam name="T">The type of items to sort.</typeparam>
public sealed class SortBuilder<T>
{
    private readonly List<ISortableParameter<T>> _sortableParameters = new();

    /// <summary>
    /// Creates a new <see cref="SortParameterBuilder{T, TProperty}"/> for the specified property.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property to sort.</typeparam>
    /// <param name="expression">The lambda expression that represents the property to sort.</param>
    /// <returns>A new <see cref="SortParameterBuilder{T, TProperty}"/> instance.</returns>
    public SortParameterBuilder<T, TProperty> ForParameter<TProperty>(Expression<Func<T, TProperty>> expression)
    {
        var parameter = new SortableParameter<T, TProperty>(expression);
        _sortableParameters.Add(parameter);
        return new SortParameterBuilder<T, TProperty>(parameter);
    }

    internal List<ISortableParameter<T>> Build() 
        => _sortableParameters;
}
