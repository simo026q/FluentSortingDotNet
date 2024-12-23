namespace FluentSortingDotNet.Queries;

/// <inheritdoc />
public sealed class DefaultSortQueryBuilderFactory<T> : ISortQueryBuilderFactory<T>
{
    /// <summary>
    /// Gets the singleton instance of the <see cref="DefaultSortQueryBuilderFactory{T}"/> class.
    /// </summary>
    public static readonly DefaultSortQueryBuilderFactory<T> Instance = new();

    private DefaultSortQueryBuilderFactory()
    {
    }

    /// <inheritdoc />
    public ISortQueryBuilder<T> Create() => new DefaultSortQueryBuilder<T>();
}
