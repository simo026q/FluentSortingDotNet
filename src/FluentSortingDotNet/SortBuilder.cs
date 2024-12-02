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
    private readonly List<SortableParameter> _sortableParameters = new();

    /// <summary>
    /// Creates a new <see cref="SortParameterBuilder"/> for the specified property.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property to sort.</typeparam>
    /// <param name="expression">The lambda expression that represents the property to sort.</param>
    /// <param name="name">The name of the parameter.</param>
    /// <returns>A new <see cref="SortParameterBuilder"/> instance.</returns>
    public SortParameterBuilder ForParameter<TProperty>(Expression<Func<T, TProperty>> expression, string name)
    {
        var parameter = new SortableParameter(expression, name);
        _sortableParameters.Add(parameter);
        return new SortParameterBuilder(parameter);
    }

    /// <summary>
    /// Creates a new <see cref="SortParameterBuilder"/> for the specified property.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property to sort.</typeparam>
    /// <param name="expression">The lambda expression that represents the property to sort.</param>
    /// <returns>A new <see cref="SortParameterBuilder"/> instance.</returns>
    public SortParameterBuilder ForParameter<TProperty>(Expression<Func<T, TProperty>> expression)
    {
        var name = GetNameFromExpression(expression);
        return ForParameter(expression, name);
    }

    private static string GetNameFromExpression<TProperty>(Expression<Func<T, TProperty>> expression)
    {
        var bodyString = expression.Body.ToString();
        var prefix = expression.Parameters[0].Name + ".";

        return bodyString.StartsWith(prefix)
            ? bodyString.Substring(prefix.Length)
            : bodyString;
    }

    internal List<SortableParameter> Build() 
        => _sortableParameters;
}
