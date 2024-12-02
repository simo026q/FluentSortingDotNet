using System;

namespace FluentSortingDotNet.Internal;

internal static class ThrowHelper
{
    public static T ThrowIfNull<T>(T? value, string? paramName)
        where T : class
        => value ?? throw new ArgumentNullException(paramName);

    public static T ThrowIfNull<T>(T? value, string? paramName, string? message)
        where T : class
        => value ?? throw new ArgumentNullException(paramName, message);
}