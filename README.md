# FluentSortingDotNet

## Example

```csharp
public record Person(string Name, int Age);
```

```csharp
using FluentSortingDotNet;
using FluentSortingDotNet.Parser;

public sealed class PersonSorter(ISortParameterParser parser) : ParsingSorter<Person>(parser)
{
    protected override void Configure(SortBuilder<Person> builder)
    {
        builder.ForParameter(x => x.Name).Name("name").Default(SortDirection.Descending);
        builder.ForParameter(x => x.Age).Name("age");
    }
}
```

```csharp
using FluentSortingDotNet;
using FluentSortingDotNet.Parser;

var parser = new DefaultSortParameterParser();
var sorter = new PersonSorter(parser);

IQueryable<Person> people = ...;

SortResult<Person> result = sorter.Sort(people, "name,-age");

if (result.IsValid)
{
    var orderedPeople = result.Query.ToList();
}
else 
{
    // Handle error
    // result.InvalidSortParameters
}
```

### Dependency Injection

```csharp
services.AddSingleton<ISortParameterParser, DefaultSortParameterParser>();
services.AddSingleton<PersonSorter>();
```