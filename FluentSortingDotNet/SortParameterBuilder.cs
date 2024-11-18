using FluentSortingDotNet.Internal;

namespace FluentSortingDotNet;

public sealed class SortParameterBuilder<T, TProperty>
{
    private readonly SortableParameter<T, TProperty> _parameter;

    internal SortParameterBuilder(SortableParameter<T, TProperty> parameter)
    {
        _parameter = parameter;
    }

    public SortParameterBuilder<T, TProperty> Default(SortDirection sortDirection)
    {
        _parameter.DefaultDirection = sortDirection;
        return this;
    }

    public SortParameterBuilder<T, TProperty> Required()
    {
        _parameter.IsRequired = true;
        return this;
    }

    public SortParameterBuilder<T, TProperty> Name(string name)
    {
        _parameter.Name = name;
        return this;
    }
}
