using FluentSortingDotNet.Queries;
using FluentSortingDotNet.Testing;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FluentSortingDotNet.Tests.Queries;

public abstract class SortQueryBuilder_SortQueryTest
{
    private readonly PersonDbContext _dbContext;

    public SortQueryBuilder_SortQueryTest()
    {
        var options = new DbContextOptionsBuilder<PersonDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new PersonDbContext(options);
    }

    protected abstract ISortQueryBuilder<Person> CreateSortQueryBuilder();

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(10)]
    public void Apply_ReturnsOrderedQuery_WhenSingleSortExpressionHaveBeenAdded(int count)
    {
        // Arrange
        var queryBuilder = CreateSortQueryBuilder();
        Expression<Func<Person, string>> lambdaExpression = x => x.Name;
        queryBuilder.SortBy(lambdaExpression, SortDirection.Ascending);

        var people = PersonGenerator.Instance.UseSeed(2025).Generate(count);

        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();
        _dbContext.People.AddRange(people);
        _dbContext.SaveChanges();

        var sortQuery = queryBuilder.Build();

        // Act
        var result = sortQuery.Apply(_dbContext.People.AsQueryable()).ToList();

        // Assert
        var expected = people.OrderBy(x => x.Name).ToList();

        Assert.NotNull(result);
        Assert.Equal(count, result.Count);

        for (int i = 0; i < count; i++)
        {
            Assert.Equal(expected[i].Name, result[i].Name);
        }
    }

    [Fact]
    public void Apply_ReturnsOrderedQuery_WhenMultipleSortExpressionsHaveBeenAdded()
    {
        // Arrange
        var queryBuilder = CreateSortQueryBuilder();
        Expression<Func<Person, string>> lambdaExpression1 = x => x.Name;
        Expression<Func<Person, DateTimeOffset>> lambdaExpression2 = x => x.DateOfBirth;
        queryBuilder.SortBy(lambdaExpression1, SortDirection.Ascending);
        queryBuilder.SortBy(lambdaExpression2, SortDirection.Descending);

        List<Person> people = [
            new Person("John", new DateTimeOffset(1990, 1, 1, 0, 0, 0, TimeSpan.Zero)),
            new Person("John", new DateTimeOffset(1995, 1, 1, 0, 0, 0, TimeSpan.Zero)),
            new Person("Alice", new DateTimeOffset(1992, 1, 1, 0, 0, 0, TimeSpan.Zero)),
        ];

        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();
        _dbContext.People.AddRange(people);
        _dbContext.SaveChanges();

        var sortQuery = queryBuilder.Build();

        // Act
        var result = sortQuery.Apply(_dbContext.People.AsQueryable()).ToList();

        // Assert
        var expected = people.OrderBy(x => x.Name).ThenByDescending(x => x.DateOfBirth).ToList();

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal(expected[0].Name, result[0].Name);
        Assert.Equal(expected[0].DateOfBirth, result[0].DateOfBirth);
        Assert.Equal(expected[1].Name, result[1].Name);
        Assert.Equal(expected[1].DateOfBirth, result[1].DateOfBirth);
        Assert.Equal(expected[2].Name, result[2].Name);
        Assert.Equal(expected[2].DateOfBirth, result[2].DateOfBirth);
    }
}