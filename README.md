# FluentSortingDotNet

## Features

- Parse sort parameters from a string in the format `name,-age`
    - A custom parser can be added by extending the `SortParameterParser` class or implementing the `ISortParameterParser` interface.
- Sort an `IQueryable<T>` based on the parsed parameters

## Extensibility

The codebase is intentionally not very extensible. This is because the library is in a early stage and I want to see how it is used before adding more features. If you have any suggestions, please open an issue.

## Example

```csharp
public record Person(string Name, int Age);
```

```csharp
using FluentSortingDotNet;
using FluentSortingDotNet.Parser;

public sealed class PersonSorter(ISortParameterParser parser) : Sorter<Person>(parser)
{
    protected override void Configure(SortBuilder<Person> builder)
    {
        builder.ForParameter(p => p.Name).Name("name").Default(SortDirection.Descending);
        builder.ForParameter(p => p.Age).Name("age");
    }
}
```

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