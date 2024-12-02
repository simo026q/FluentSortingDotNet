# FluentSortingDotNet

## Features

- Parse sort parameters from a string in the format `name,-age`
    - A custom parser can be added by extending the `SortParameterParser` class or implementing the `ISortParameterParser` interface.
- Sort an `IQueryable<T>` based on the parsed parameters
- Handle invalid sort parameters

## Extensibility

The codebase is intentionally not very extensible. This is because the library is in a early stage and I want to see how it is used before adding more features. If you have any suggestions, please open an issue.

## Example

### Entity

```csharp
public record Person(string Name, int Age);
```

### Sorter

```csharp
using FluentSortingDotNet;
using FluentSortingDotNet.Parser;

public sealed class PersonSorter(ISortParameterParser parser) : Sorter<Person>(parser)
{
    protected override void Configure(SortBuilder<Person> builder)
    {
        // when no parameters are provided, sort by name descending
        builder.ForParameter(p => p.Name).Name("name").Default(SortDirection.Descending);

        builder.ForParameter(p => p.Age).Name("age");
    }
}
```

### Usage

```csharp
using FluentSortingDotNet;
using FluentSortingDotNet.Parser;

var parser = new DefaultSortParameterParser();
var sorter = new PersonSorter(parser);

IQueryable<Person> peopleQuery = ...;

SortResult result = sorter.Sort(ref peopleQuery, "name,-age");

if (result.IsSuccess)
{
    var orderedPeople = peopleQuery.ToList();
}
else 
{
    Console.WriteLine($"Invalid sort parameters: {string.Join(", ", result.InvalidSortParameters)}");
}
```

### Dependency Injection

```csharp
services.AddSingleton<ISortParameterParser, DefaultSortParameterParser>();
services.AddSingleton<PersonSorter>();
```

## Performance

The library is designed to be fast and memory efficient. The area that is yet to be optimized is the reflection used to call all the `OrderBy` methods.

### Benchmarks

See the [benchmarks](tests/FluentSortingDotNet.Benchmarks/SorterBenchmarks.cs) for more information. Take the benchmark results with a grain of salt, as they seem to be too good to be true.

![Benchmark results](tests/FluentSortingDotNet.Benchmarks/v1.0.0-beta.2.png "Benchmark results")