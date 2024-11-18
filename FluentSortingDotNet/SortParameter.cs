namespace FluentSortingDotNet;

public readonly struct SortParameter(string name, SortDirection direction)
{
    public string Name { get; } = name;
    public SortDirection Direction { get; } = direction;
}
