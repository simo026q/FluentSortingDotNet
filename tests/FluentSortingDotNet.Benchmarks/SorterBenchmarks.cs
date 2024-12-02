using BenchmarkDotNet.Attributes;
using FluentSortingDotNet;
using FluentSortingDotNet.Parser;

[MemoryDiagnoser(false)]
public class SorterBenchmarks
{
    public static readonly IQueryable<Person> People = Enumerable.Range(0, 10).Select(i => new Person($"Name{i + 1}", i * 2 + 18)).ToArray().AsQueryable();
    public static readonly PersonSorter Sorter = new();

    public const string EmptyQuery = "";
    public const string FullQuery = "name,-age";
    public const string InvalidQuery = "name,-invalid";

    [Benchmark]
    public List<Person> Sort_EmptyQuery()
    {
        IQueryable<Person> queryCopy = People;
        Sorter.Sort(ref queryCopy, EmptyQuery);
        return queryCopy.ToList();
    }

    [Benchmark]
    public List<Person> Sort_EmptyQuery_Linq()
    {
        return People.OrderByDescending(p => p.Name).ToList();
    }

    [Benchmark]
    public List<Person> Sort_FullQuery()
    {
        IQueryable<Person> queryCopy = People;
        Sorter.Sort(ref queryCopy, FullQuery);
        return queryCopy.ToList();
    }

    [Benchmark]
    public List<Person> Sort_FullQuery_Linq()
    {
        return People.OrderBy(p => p.Name).ThenByDescending(p => p.Age).ToList();
    }

    [Benchmark]
    public List<string> Sort_InvalidQuery()
    {
        IQueryable<Person> queryCopy = People;
        SortResult result = Sorter.Sort(ref queryCopy, InvalidQuery);
        return result.InvalidSortParameters.ToList();
    }

    public sealed record Person(string Name, int Age);

    public sealed class PersonSorter : Sorter<Person>
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