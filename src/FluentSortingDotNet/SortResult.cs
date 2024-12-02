using FluentSortingDotNet.Internal;
using System.Collections.Generic;
using System.Linq;

namespace FluentSortingDotNet;

/// <summary>
/// Represents the result of a sort operation.
/// </summary>
public readonly struct SortResult
{
    private static readonly IEnumerable<string> EmptySortParameters = Enumerable.Empty<string>();
    private static readonly SortResult SuccessResult = new(true, null);

    /// <remarks>Only <see langword="null"/> when <see cref="SortResult"/> is <see langword="default"/></remarks>
    private readonly IEnumerable<string>? _invalidSortParameters;

    /// <summary>
    /// Gets a value indicating whether the sort operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the sort operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets a collection of invalid sort parameters.
    /// </summary>
    public IEnumerable<string> InvalidSortParameters => _invalidSortParameters ?? EmptySortParameters;

    private SortResult(bool isSuccess, IEnumerable<string>? invalidSortParameters)
    {
        IsSuccess = isSuccess;
        _invalidSortParameters = invalidSortParameters ?? EmptySortParameters;
    }

    /// <summary>
    /// Gets a successful <see cref="SortResult"/>.
    /// </summary>
    /// <returns>Returns a successful <see cref="SortResult"/>.</returns>
    public static SortResult Success()
        => SuccessResult;

    /// <summary>
    /// Creates a new <see cref="SortResult"/> that represents a failed sort operation.
    /// </summary>
    /// <param name="invalidSortParameters">A collection of invalid sort parameters.</param>
    /// <returns>Returns a new <see cref="SortResult"/> that represents a failed sort operation.</returns>
    public static SortResult Failure(IEnumerable<string> invalidSortParameters)
        => new(false, ThrowHelper.ThrowIfNull(invalidSortParameters, nameof(invalidSortParameters)));
}