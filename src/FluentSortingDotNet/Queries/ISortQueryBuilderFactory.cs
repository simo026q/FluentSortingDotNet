namespace FluentSortingDotNet.Queries;

/// <summary>
/// Represents a factory for creating instances of <see cref="ISortQueryBuilder{T}"/>.
/// </summary>
/// <typeparam name="T">The type of items to sort.</typeparam>
public interface ISortQueryBuilderFactory<T>
{
    /// <summary>
    /// Creates a new instance of <see cref="ISortQueryBuilder{T}"/>.
    /// </summary>
    /// <returns>Returns a new instance of <see cref="ISortQueryBuilder{T}"/>.</returns>
    ISortQueryBuilder<T> Create();
}