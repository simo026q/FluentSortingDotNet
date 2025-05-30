using BenchmarkDotNet.Attributes;
using FluentSortingDotNet;
using FluentSortingDotNet.Queries;
using FluentSortingDotNet.Testing;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

[MemoryDiagnoser(true), MarkdownExporter]
public class QueryBuilderBenchmarks
{
    public static readonly List<Person> People = PersonGenerator.Instance.UseSeed(2024).Generate(10);
    public static readonly PersonDbContext DbContext;

    static QueryBuilderBenchmarks()
    {
        var options = new DbContextOptionsBuilder<PersonDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .UseSeeding((db, _) => db.AddRange(People))
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .Options;

        DbContext = new PersonDbContext(options);
    }

    public static readonly LambdaExpression NameExpression = (Expression<Func<Person, string>>)(p => p.Name);
    public static readonly LambdaExpression DateOfBirthExpression = (Expression<Func<Person, DateTimeOffset>>)(p => p.DateOfBirth);

    public static readonly DefaultSortQueryBuilderFactory<Person> DefaultQueryBuilderFactory = DefaultSortQueryBuilderFactory<Person>.Instance;
    public static readonly ISortQuery<Person> ExpressionSortQuery = new ExpressionSortQueryBuilder<Person>().SortBy(NameExpression, SortDirection.Ascending).SortBy(DateOfBirthExpression, SortDirection.Descending).Build();

    public static readonly Func<PersonDbContext, IEnumerable<Person>> CompiledQuery = EF.CompileQuery<PersonDbContext, IEnumerable<Person>>(dbContext => dbContext.People.OrderBy(p => p.Name).ThenByDescending(p => p.DateOfBirth));

    [Benchmark]
    public List<Person> QueryBuilder_Default()
    {
        return DefaultQueryBuilderFactory
            .Create()
            .SortBy(NameExpression, SortDirection.Ascending)
            .SortBy(DateOfBirthExpression, SortDirection.Descending)
            .Build()
            .Apply(DbContext.People)
            .ToList();
    }

    [Benchmark]
    public List<Person> QueryBuilder_Expression()
    {
        return ExpressionSortQuery.Apply(DbContext.People).ToList();
    }

    [Benchmark(Baseline = true)]
    public List<Person> EF_Linq()
    {
        return DbContext
            .People
            .OrderBy(p => p.Name)
            .ThenByDescending(p => p.DateOfBirth)
            .ToList();
    }

    [Benchmark]
    public List<Person> EF_Compiled()
    {
        return CompiledQuery(DbContext).ToList();
    }
}
