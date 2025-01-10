﻿using FluentSortingDotNet.Queries;
using FluentSortingDotNet.Testing;

namespace FluentSortingDotNet.UnitTests.Queries;

public class DefaultSortQueryBuilderTests
{
    [Fact]
    public void Build_WhenNoSortExpressionsHaveBeenAdded_ThrowsInvalidOperationException()
    {
        // Arrange
        var queryBuilder = new DefaultSortQueryBuilder<object>();

        // Act
        Action act = () => queryBuilder.Build();

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void Apply_WhenNotBuilt_ThrowsInvalidOperationException()
    {
        // Arrange
        var query = Enumerable.Empty<object>().AsQueryable();
        var queryBuilder = new DefaultSortQueryBuilder<object>();

        // Act
        Action act = () => queryBuilder.Apply(query);

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void Apply_WhenSortExpressionsHaveBeenAdded_ReturnsOrderedQuery()
    {
        // Arrange
        Person[] data = [new Person { Age = 18, Name = "Alice" }, new Person { Age = 25, Name = "Bob" }, new Person { Age = 30, Name = "Charlie" }];
        IQueryable<Person> query = data.AsQueryable();
        var queryBuilder = new DefaultSortQueryBuilder<Person>();
        queryBuilder.SortBy((Person p) => p.Name, SortDirection.Descending);

        // Act
        var orderedQuery = queryBuilder.Build().Apply(query);

        // Assert
        var result = orderedQuery.ToList();
        Assert.Equal("Charlie", result[0].Name);
        Assert.Equal("Bob", result[1].Name);
        Assert.Equal("Alice", result[2].Name);
    }
}