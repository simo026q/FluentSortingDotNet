using BenchmarkDotNet.Attributes;
using FluentSortingDotNet;
using FluentSortingDotNet.Queries;
using FluentSortingDotNet.Testing;
using System.Linq.Expressions;

[MemoryDiagnoser(false)]
public class QueryBuilderBenchmarks
{
    public static readonly IQueryable<Person> People = PersonGenerator.Instance.UseSeed(2024).Generate(10).AsQueryable();

    public static readonly LambdaExpression NameExpression = (Expression<Func<Person, string>>)(p => p.Name);
    public static readonly LambdaExpression AgeExpression = (Expression<Func<Person, int>>)(p => p.Age);

    public static readonly DefaultSortQueryBuilderFactory<Person> DefaultQueryBuilderFactory = DefaultSortQueryBuilderFactory<Person>.Instance;
    public static readonly ISortQuery<Person> CompiledSortQuery = new ExpressionSortQueryBuilder<Person>().SortBy(NameExpression, SortDirection.Ascending).SortBy(AgeExpression, SortDirection.Descending).Build();

    [Benchmark]
    public List<Person> Default()
    {
        return DefaultQueryBuilderFactory
            .Create()
            .SortBy(NameExpression, SortDirection.Ascending)
            .SortBy(AgeExpression, SortDirection.Descending)
            .Build()
            .Apply(People)
            .ToList();
    }

    [Benchmark]
    public List<Person> Compiled()
    {
        return CompiledSortQuery.Apply(People).ToList();
    }

    [Benchmark(Baseline = true)]
    public List<Person> Linq()
    {
        return People
            .OrderBy(p => p.Name)
            .ThenByDescending(p => p.Age)
            .ToList();
    }
}
