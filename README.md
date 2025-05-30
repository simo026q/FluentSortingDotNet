# FluentSortingDotNet

## Installation

```bash
Install-Package FluentSortingDotNet
```

## Versioning

| Version | Description                                                                                        | API Changes |
|---------|----------------------------------------------------------------------------------------------------|-------------|
| Major   | Big breaking changes, new features, and improvements                                               | Yes         |
| Minor   | New features, improvements, minor breaking changes (e.g. renaming, removing, or adding parameters) | Yes         |
| Patch   | Bug fixes, performance improvements, and minor changes                                             | No          |

## Features

- Parse sort parameters from a string in the format `name,-age`
    - A custom parser can be added by extending the `SortParameterParser` class or implementing the `ISortParameterParser` interface.
- Sort an `IQueryable<T>` based on the parsed parameters
- Handle invalid sort parameters

## Example

### Entity

```csharp
public record Person(string Name, int Age);
```

### Sorter

```csharp
using FluentSortingDotNet;

public sealed class PersonSorter : Sorter<Person>
{
    protected override void Configure(SortBuilder<Person> builder)
    {
        // When no parameters are provided, sort by name descending
        builder.ForParameter(p => p.Name).IsDefault(direction: SortDirection.Descending);

        builder.ForParameter(p => p.DateOfBirth).WithName("age").ReverseDirection();

        // Ignore case when sorting by name
        builder.IgnoreParameterCase();

        // Ignore invalid parameters instead of throwing an exception when not validated with PersonSorter.Validate(string)
        builder.IgnoreInvalidParameters();
    }
}
```

### Usage

```csharp
using FluentSortingDotNet;

PersonSorter sorter = new();

SortContext sortContext = sorter.Validate("name,-age");

if (!sortContext.IsValid) 
{
    Console.WriteLine($"Invalid sort parameters: {string.Join(", ", sortContext.InvalidParameters)}");
    return;
}

IQueryable<Person> peopleQuery = ...;

IQueryable<Person> sortedQuery = sorter.Sort(peopleQuery, sortContext);
```

### Dependency Injection

```csharp
services.AddSingleton<ISorter<Person>, PersonSorter>();
```

## Extensibility

Extensibility will be improved hand in hand with the stability of the library. The API is currently subject to breaking changes. If you have any suggestions, please open an issue.

### Custom Sort Parameter Parser

To create a custom sort parameter parser, extend the `SortParameterParser` class or implement the `ISortParameterParser` interface.

#### Example

```csharp
using FluentSortingDotNet;
using FluentSortingDotNet.Parser;

// Parse a query in the format `name.asc,age.desc`
public sealed class CustomSortParameterParser : SortParameterParser
{
    protected override int IndexOfSeparator(ReadOnlySpan<char> query)
        => query.IndexOf(',');

    public override bool TryParseParameter(ReadOnlySpan<char> parameter, out SortParameter sortParameter)
    {
        SortDirection direction = SortDirection.Ascending;

        if (parameter.IsEmpty)
        {
            sortParameter = SortParameter.Empty;
            return false;
        }

        var directionSeperatorIndex = parameter.IndexOf('.');
        if (directionSeperatorIndex == -1)
        {
            sortParameter = SortParameter.Empty;
            return false;
        }

        var parameterName = parameter.Slice(0, directionSeperatorIndex).ToString();
        var directionName = parameter.Slice(directionSeperatorIndex + 1).ToString();

        switch (directionName)
        {
            case "asc":
                direction = SortDirection.Ascending;
                break;
            case "desc":
                direction = SortDirection.Descending;
                break;
            default:
                sortParameter = SortParameter.Empty;
                return false;
        }

        sortParameter = new SortParameter(parameterName, direction);
        return true;
    }
}
```

##### Usage
```csharp
using FluentSortingDotNet;

public sealed class PersonSorter() : Sorter<Person>(new CustomSortParameterParser())
{
    // Code omitted for brevity
}
```

### Custom Query Builder

To create a custom query builder, implement the `ISortQueryBuilder` interface along with a `ISortQueryBuilderFactory` that creates the query builder. This will rarely be needed since the default query builder is very efficient. By default the `DefaultSortQueryBuilderFactory<T>` is used to create dynamic queries and the `ExpressionSortQueryBuilder<T>` is used to create precompiled queries for the default sort parameters.

## Performance

The library is designed to be fast and memory efficient. The area that is yet to be optimized is the reflection used to call all the `OrderBy` methods.

### Benchmarks

#### Query building

The query building is very fast. 
It has a slightly worse performance (when using a sort query string) than calling the `OrderBy`, `OrderByDescending`, `ThenBy`, and `ThenByDescending` methods directly. 
The performance is slightly better when sorting on the default sort parameters since the query is precompiled.
Both of the benchmarked query builders allocate a bit less memory since the expressions are reused.

| Method                  | Mean     | Error     | StdDev    | Ratio | Gen0   | Gen1   | Allocated | Alloc Ratio |
|------------------------ |---------:|----------:|----------:|------:|-------:|-------:|----------:|------------:|
| QueryBuilder_Default    | 3.901 μs | 0.0114 μs | 0.0095 μs |  0.86 | 0.4501 |      - |   4.62 KB |        0.78 |
| QueryBuilder_Expression | 3.709 μs | 0.0121 μs | 0.0107 μs |  0.82 | 0.4387 | 0.0038 |    4.5 KB |        0.76 |
| EF_Linq                 | 4.549 μs | 0.0174 μs | 0.0154 μs |  1.00 | 0.5798 |      - |   5.94 KB |        1.00 |
| EF_Compiled             | 1.434 μs | 0.0040 μs | 0.0033 μs |  0.32 | 0.1335 |      - |   1.38 KB |        0.23 |

#### Parsing

The parsing has no real-world impact on performance.

| Method     | Query            | Mean     | Error     | StdDev    | Allocated |
|----------- |----------------- |---------:|----------:|----------:|----------:|
| ParseFirst | -a,b             | 6.893 ns | 0.0395 ns | 0.0370 ns |      24 B |
| ParseFirst | a,-b             | 6.203 ns | 0.0139 ns | 0.0130 ns |      24 B |
| ParseFirst | a,b,-c,d,-e,-f,g | 6.442 ns | 0.0276 ns | 0.0230 ns |      24 B |
