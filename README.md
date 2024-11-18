# FluentSortingDotNet

## Example

```csharp
public record Person(string Name, int Age);
```

```csharp
using FluentSortingDotNet;
using FluentSortingDotNet.Parser;

public sealed class PersonSorter : ParsingSorter<Person>
{
    public PersonSorter(ISortParameterParser parser) : base(parser)
    {
        ForParameter(p => p.Name).Name("name").Default(SortDirection.Descending);
        ForParameter(p => p.Age).Name("age");
    }
}
```

```csharp
using FluentSortingDotNet;
using FluentSortingDotNet.Parser;

var parser = new DefaultSortParameterParser();
var sorter = new PersonSorter(parser);

IQueryable<Person> people = ...;

var orderedPeople = sorter.Sort(people, "name,-age");
```

### Dependency Injection

```csharp
services.AddSingleton<ISortParameterParser, DefaultSortParameterParser>();
services.AddSingleton<PersonSorter>();
```