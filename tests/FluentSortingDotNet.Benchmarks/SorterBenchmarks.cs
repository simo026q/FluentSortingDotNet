using BenchmarkDotNet.Attributes;
using FluentSortingDotNet;
using FluentSortingDotNet.Parser;

[MemoryDiagnoser(false)]
public class SorterBenchmarks
{
    public static readonly IQueryable<Person> People = Enumerable.Range(0, 10).Select(i => new Person($"Name{i + 1}", i * 2 + 18)).ToArray().AsQueryable();
    public sealed record Person(string Name, int Age);

    private const string EmptyQuery = "";
    private const string FullQuery = "name,-age";
    private const string InvalidQuery = "name,-invalid";

    private static readonly PersonSorter Sorter = new();

    [Benchmark]
    public SortResult Sort_NoSortParameters()
    {
        IQueryable<Person> queryCopy = People;
        return Sorter.Sort(ref queryCopy, EmptyQuery);
    }

    [Benchmark]
    public SortResult Sort_FullSortParameters()
    {
        IQueryable<Person> queryCopy = People;
        return Sorter.Sort(ref queryCopy, FullQuery);
    }

    [Benchmark]
    public SortResult Sort_InvalidSortParameters()
    {
        IQueryable<Person> queryCopy = People;
        return Sorter.Sort(ref queryCopy, InvalidQuery);
    }

    private sealed class PersonSorter : Sorter<Person>
    {
        public PersonSorter() : base(new DefaultSortParameterParser())
        { }

        protected override void Configure(SortBuilder<Person> builder)
        {
            builder.ForParameter(x => x.Name).Name("name").Default(SortDirection.Descending);
            builder.ForParameter(x => x.Age).Name("age");
        }
    }
}