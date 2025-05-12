using System.Diagnostics;

namespace FluentSortingDotNet.Testing;

[DebuggerDisplay("{Name} ({Age})")]
public sealed class Person
{
    public Person()
    {
    }

    public Person(string name, DateTimeOffset dateOfBirth)
    {
        Name = name;
        DateOfBirth = dateOfBirth;
    }

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset DateOfBirth { get; set; }
    public int Age => DateTimeOffset.UtcNow.Year - DateOfBirth.Year;
}