namespace FluentSortingDotNet.Internal;

internal sealed class CombinedSortParameter<T>(ISortableParameter<T> sortableParameter, SortParameter sortParameter)
{
    public ISortableParameter<T> SortableParameter { get; } = sortableParameter;
    public SortParameter SortParameter { get; } = sortParameter;
}