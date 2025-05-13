using System;
using System.Linq;

namespace FluentSortingDotNet;

/// <summary>
/// Extensions for the <see cref="ISorter{T}"/> interface.
/// </summary>
public static class SorterExtensions
{
    /// <summary>
    /// Sorts the specified query using the specified <see cref="SortContext{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of items to sort.</typeparam>
    /// <param name="sorter">The sorter to use for sorting.</param>
    /// <param name="query">The query to sort.</param>
    /// <param name="sortContext">The <see cref="SortContext{T}"/> that contains the sort parameters.</param>
    /// <returns>A new query with the sorting applied.</returns>
    public static IQueryable<T> Sort<T>(this ISorter<T> sorter, IQueryable<T> query, SortContext<T> sortContext)
        => sorter.CreateSortQuery(sortContext).Apply(query);

    /// <summary>
    /// Sorts the specified query using the specified sort query.
    /// </summary>
    /// <typeparam name="T">The type of items to sort.</typeparam>
    /// <param name="sorter">The sorter to use for sorting.</param>
    /// <param name="query">The query to sort.</param>
    /// <param name="sortQuery">The sort query to use for sorting.</param>
    /// <returns>A new query with the sorting applied.</returns>
    public static IQueryable<T> Sort<T>(this ISorter<T> sorter, IQueryable<T> query, ReadOnlySpan<char> sortQuery) 
        => sorter.Sort(query, sorter.Validate(sortQuery));

    /// <summary>
    /// Sorts the specified query using the specified sort query.
    /// </summary>
    /// <typeparam name="T">The type of items to sort.</typeparam>
    /// <param name="sorter">The sorter to use for sorting.</param>
    /// <param name="query">The query to sort.</param>
    /// <param name="sortQuery">The sort query to use for sorting.</param>
    /// <returns>A new query with the sorting applied.</returns>
    public static IQueryable<T> Sort<T>(this ISorter<T> sorter, IQueryable<T> query, string? sortQuery) 
        => sorter.Sort(query, sortQuery.AsSpan()); // when sortQuery is null, AsSpan() will return default(ReadOnlySpan<char>)

    /// <summary>
    /// Sorts the specified query using the default sort parameters.
    /// </summary>
    /// <typeparam name="T">The type of items to sort.</typeparam>
    /// <param name="sorter">The sorter to use for sorting.</param>
    /// <param name="query">The query to sort.</param>
    /// <returns>A new query with the default sorting applied.</returns>
    public static IQueryable<T> Sort<T>(this ISorter<T> sorter, IQueryable<T> query) 
        => sorter.Sort(query, SortContext<T>.Empty);

    /// <summary>
    /// Sorts the specified query using the specified <see cref="ISorter{T}"/> and <see cref="SortContext{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of items to sort.</typeparam>
    /// <param name="query">The query to sort.</param>
    /// <param name="sorter">The sorter to use for sorting.</param>
    /// <param name="sortContext">The <see cref="SortContext{T}"/> that contains the sort parameters.</param>
    /// <returns>A new query with the sorting applied.</returns>
    public static IQueryable<T> UseSorter<T>(this IQueryable<T> query, ISorter<T> sorter, SortContext<T> sortContext)
        => sorter.Sort(query, sortContext);

    /// <summary>
    /// Sorts the specified query using the specified <see cref="ISorter{T}"/> and sort query.
    /// </summary>
    /// <typeparam name="T">The type of items to sort.</typeparam>
    /// <param name="query">The query to sort.</param>
    /// <param name="sorter">The sorter to use for sorting.</param>
    /// <param name="sortQuery">The sort query to use for sorting.</param>
    /// <returns>A new query with the sorting applied.</returns>
    public static IQueryable<T> UseSorter<T>(this IQueryable<T> query, ISorter<T> sorter, ReadOnlySpan<char> sortQuery)
        => sorter.Sort(query, sortQuery);

    /// <summary>
    /// Sorts the specified query using the specified <see cref="ISorter{T}"/> and sort query.
    /// </summary>
    /// <typeparam name="T">The type of items to sort.</typeparam>
    /// <param name="query">The query to sort.</param>
    /// <param name="sorter">The sorter to use for sorting.</param>
    /// <param name="sortQuery">The sort query to use for sorting.</param>
    /// <returns>A new query with the sorting applied.</returns>
    public static IQueryable<T> UseSorter<T>(this IQueryable<T> query, ISorter<T> sorter, string? sortQuery)
        => sorter.Sort(query, sortQuery.AsSpan()); // when sortQuery is null, AsSpan() will return default(ReadOnlySpan<char>)

    /// <summary>
    /// Sorts the specified query using the specified <see cref="ISorter{T}"/> with default sort parameters.
    /// </summary>
    /// <typeparam name="T">The type of items to sort.</typeparam>
    /// <param name="query">The query to sort.</param>
    /// <param name="sorter">The sorter to use for sorting.</param>
    /// <returns>A new query with the sorting applied.</returns>
    public static IQueryable<T> UseSorter<T>(this IQueryable<T> query, ISorter<T> sorter)
        => sorter.Sort(query);

    /// <summary>
    /// Validates the specified sort query and returns a <see cref="SortContext{T}"/> that can be used to sort a query.
    /// </summary>
    /// <typeparam name="T">The type of items to sort.</typeparam>
    /// <param name="sorter">The sorter to use for validation.</param>
    /// <param name="sortQuery">The sort query to validate.</param>
    /// <returns>A new <see cref="SortContext{T}"/> that contains the valid and invalid parameters.</returns>
    public static SortContext<T> Validate<T>(this ISorter<T> sorter, string? sortQuery) 
        => sorter.Validate(sortQuery.AsSpan()); // when sortQuery is null, AsSpan() will return default(ReadOnlySpan<char>)
}
