using Bogus;

namespace FluentSortingDotNet.Testing;

public sealed class PersonGenerator : Faker<Person>
{
    public static readonly PersonGenerator Instance = new();

    private PersonGenerator()
    {
        RuleFor(p => p.Name, f => f.Person.FullName);
        RuleFor(p => p.DateOfBirth, f => f.Person.DateOfBirth);
    }
}