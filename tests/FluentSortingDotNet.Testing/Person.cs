using Bogus;

namespace FluentSortingDotNet.Testing;

public sealed class Person
{
    public static readonly Faker<Person> Faker = new Faker<Person>()
        .RuleFor(p => p.Name, f => f.Person.FullName)
        .RuleFor(p => p.Age, f => f.Random.Int(18, 65));

    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }

    public override string ToString() => $"{Name} ({Age})";
}