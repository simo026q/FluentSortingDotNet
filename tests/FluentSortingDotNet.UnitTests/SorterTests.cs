using FluentSortingDotNet.Testing;
using Microsoft.EntityFrameworkCore;

namespace FluentSortingDotNet.UnitTests;

public class SorterTests
{
    private sealed class PeopleContext(DbContextOptions<PeopleContext> options) : DbContext(options)
    {
        public DbSet<Person> People { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>().HasKey(p => p.Name);
        }
    }

    private readonly PeopleContext _dbContext;
    private readonly List<Person> _people;
    private readonly PersonSorter _sorter = new();

    public SorterTests()
    {
        DbContextOptions<PeopleContext> options = new DbContextOptionsBuilder<PeopleContext>()
            .UseInMemoryDatabase(nameof(SorterTests))
            .Options;

        _dbContext = new PeopleContext(options);
        _people = Person.Faker.UseSeed(2024).Generate(10);
    }

    [Fact]
    public void Sort_WithValidSortParameter_SortsCorrectly()
    {
        // Arrange
        _dbContext.Database.EnsureDeleted();
        _dbContext.AddRange(_people);
        _dbContext.SaveChanges();

        // Act
        IQueryable<Person> queryable = _dbContext.People;
        SortResult result = _sorter.Sort(ref queryable, "name");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_people.OrderBy(p => p.Name).Select(p => p.Name), queryable.Select(p => p.Name));
        Assert.Empty(result.InvalidSortParameters);
    }

    [Fact]
    public void Sort_InvalidSortParameter_ReturnsInvalidSortParameters()
    {
        // Arrange
        _dbContext.Database.EnsureDeleted();
        _dbContext.AddRange(_people);
        _dbContext.SaveChanges();

        // Act
        IQueryable<Person> queryable = _dbContext.People;
        SortResult result = _sorter.Sort(ref queryable, "invalid");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(["invalid"], result.InvalidSortParameters);
    }

    [Fact]
    public void Sort_WithEmptySortQuery_SortsWithDefaultSortParameters()
    {
        // Arrange
        _dbContext.Database.EnsureDeleted();
        _dbContext.AddRange(_people);
        _dbContext.SaveChanges();

        // Act
        IQueryable<Person> queryable = _dbContext.People;
        SortResult result = _sorter.Sort(ref queryable, string.Empty);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_people.OrderByDescending(p => p.Name).Select(p => p.Name), queryable.Select(p => p.Name));
        Assert.Empty(result.InvalidSortParameters);
    }
}
