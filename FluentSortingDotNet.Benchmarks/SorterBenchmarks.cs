using BenchmarkDotNet.Attributes;
using FluentSortingDotNet;

[MemoryDiagnoser(false)]
public class SorterBenchmarks
{
    public static readonly IQueryable<Person> People = Enumerable.Range(0, 10).Select(i => new Person($"Name{i + 1}", i * 2 + 18)).ToArray().AsQueryable();
    public sealed record Person(string Name, int Age);

    private static readonly IEnumerable<SortParameter> NoSortParameters = [];

    private static readonly IEnumerable<SortParameter> FullSortParameters = [
        new SortParameter("name", SortDirection.Descending),
        new SortParameter("age", SortDirection.Ascending)
    ];

    private static readonly PersonSorter Sorter = new();

    [Benchmark]
    public SortResult<Person> Sort_NoSortParameters()
    {
        return Sorter.Sort(People, NoSortParameters);
    }

    [Benchmark]
    public SortResult<Person> Sort_FullSortParameters()
    {
        return Sorter.Sort(People, FullSortParameters);
    }

    private sealed class PersonSorter : Sorter<Person>
    {
        protected override void Configure(SortBuilder<Person> builder)
        {
            builder.ForParameter(x => x.Name).Name("name").Default(SortDirection.Descending);
            builder.ForParameter(x => x.Age).Name("age");
        }
    }
}