﻿using FluentSortingDotNet;
using FluentSortingDotNet.Parser;

var random = new Random(2024);
var people = Enumerable.Range(0, 10).Select(_ => Person.Random(random)).ToArray();

var parser = new DefaultSortParameterParser();
var sorter = new PersonSorter(parser);

Console.WriteLine("Type 'exit' to quit.");

string? query;
while ((query = GetInput("Enter sort query: ")) != "exit")
{
    if (string.IsNullOrWhiteSpace(query))
    {
        continue;
    }

    SortResult<Person> result = sorter.Sort(people.AsQueryable(), query.AsSpan());
    if (result.IsValid)
    {
        Console.WriteLine(string.Join(", ", result.Query));
    }
    else
    {
        Console.WriteLine($"Invalid sort parameters: {string.Join(", ", result.InvalidSortParameters.Select(x => $"{(x.Direction == SortDirection.Descending ? "-" : "")}{x.Name}"))}");
        Console.WriteLine($"Missing sort parameter names: {string.Join(", ", result.MissingSortParameterNames)}");
    }
}

static string? GetInput(string prompt)
{
    Console.Write(prompt);
    return Console.ReadLine();
}

public sealed class Person
{
    private static readonly string[] Names = ["Alice", "Bob", "Charlie", "David", "Eve", "Frank", "Grace", "Hannah", "Ivy", "Jack"];

    public string Name { get; set; }
    public int Age { get; set; }

    public static Person Random(Random random)
    {
        return new Person
        {
            Name = Names[random.Next(Names.Length)],
            Age = random.Next(18, 100)
        };
    }

    public override string ToString()
    {
        return $"{Name} ({Age})";
    }
}

public sealed class PersonSorter : ParsingSorter<Person>
{
    public PersonSorter(ISortParameterParser parser) : base(parser)
    {
        ForParameter(p => p.Name).Default(SortDirection.Descending);
        ForParameter(p => p.Age);
    }
}