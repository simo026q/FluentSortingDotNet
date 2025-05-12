using FluentSortingDotNet.Queries;
using FluentSortingDotNet.Testing;
using System.Linq.Expressions;

namespace FluentSortingDotNet.Tests.Queries;

public abstract class SortQueryBuilderTests
{
    protected abstract ISortQueryBuilder<Person> CreateSortQueryBuilder();

    [Fact]
    public void IsEmpty_ReturnsTrue_WhenUnsorted()
    {
        // Arrange
        var queryBuilder = CreateSortQueryBuilder();

        // Act
        bool result = queryBuilder.IsEmpty;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsEmpty_ReturnFalse_WhenSorted()
    {
        // Arrange
        var queryBuilder = CreateSortQueryBuilder();
        Expression<Func<Person, string>> lambdaExpression = x => x.Name;
        queryBuilder.SortBy(lambdaExpression, SortDirection.Ascending);

        // Act
        bool result = queryBuilder.IsEmpty;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Build_ThrowsInvalidOperationException_WhenUnsorted()
    {
        // Arrange
        var queryBuilder = CreateSortQueryBuilder();

        // Act
        Action act = () => queryBuilder.Build();

        // Assert
        Assert.True(queryBuilder.IsEmpty);
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void Build_ReturnsSortQuery_WhenSorted()
    {
        // Arrange
        var queryBuilder = CreateSortQueryBuilder();
        Expression<Func<Person, string>> lambdaExpression = x => x.Name;
        queryBuilder.SortBy(lambdaExpression, SortDirection.Ascending);

        // Act
        var result = queryBuilder.Build();

        // Assert
        Assert.False(queryBuilder.IsEmpty);
        Assert.NotNull(result);
        Assert.IsAssignableFrom<ISortQuery<Person>>(result);
    }
}