using FluentSortingDotNet;
using FluentSortingDotNet.Testing;

List<Person> people = Person.Faker.UseSeed(2024).Generate(10);
var sorter = new PersonSorter();

Console.WriteLine("Type 'exit' to quit.");

string query;
while ((query = GetInput("Enter sort query: ")) != "exit")
{
    IQueryable<Person> queryable = people.AsQueryable();
    SortResult result = sorter.Sort(ref queryable, query);
    if (result.IsSuccess)
    {
        Console.WriteLine(string.Join(", ", queryable));
    }
    else
    {
        Console.WriteLine($"Invalid sort parameters: {string.Join(", ", result.InvalidSortParameters)}");
    }
}

static string GetInput(string prompt)
{
    Console.Write(prompt);
    return Console.ReadLine() ?? string.Empty;
}