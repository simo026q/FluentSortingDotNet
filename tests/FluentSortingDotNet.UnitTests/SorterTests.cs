using FluentSortingDotNet.Parsers;
using FluentSortingDotNet.Queries;
using FluentSortingDotNet.Testing;
using NSubstitute;

namespace FluentSortingDotNet.Tests;

public class SorterTests
{
    private static SorterOptions CreateOptions(bool ignoreInvalidParameters = false, IEqualityComparer<string>? parameterNameComparer = null)
        => new() { IgnoreInvalidParameters = ignoreInvalidParameters, ParameterNameComparer = parameterNameComparer ?? StringComparer.Ordinal };

    private static PersonSorter CreateSorter(ISortParameterParser? parser = null, ISortQueryBuilderFactory<Person>? sortQueryBuilderFactory = null, ISortQueryBuilder<Person>? defaultParameterSortQueryBuilder = null, SorterOptions? options = null)
        => new(parser, sortQueryBuilderFactory, defaultParameterSortQueryBuilder, options);

    [Fact]
    public void CreateSortQuery_ShouldThrowArgumentException_WhenSortContextIsInvalid()
    {
        // Arrange
        SorterOptions options = CreateOptions(ignoreInvalidParameters: false);
        Sorter<Person> sorter = CreateSorter(options: options);
        SortContext<Person> sortContext = new(Array.Empty<SortParameter>(), ["invalid_parameter"]);

        // Act
        Action act = () => sorter.CreateSortQuery(sortContext);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void CreateSortQuery_ShouldNotThrow_WhenIgnoreInvalidParametersIsTrue()
    {
        // Arrange
        ISortQuery<Person> defaultSortQuery = Substitute.For<ISortQuery<Person>>();
        ISortQueryBuilder<Person> sortQueryBuilder = Substitute.For<ISortQueryBuilder<Person>>();
        sortQueryBuilder.IsEmpty.Returns(false);
        sortQueryBuilder.Build().Returns(defaultSortQuery);

        SorterOptions options = CreateOptions(ignoreInvalidParameters: true);

        Sorter<Person> sorter = CreateSorter(
            defaultParameterSortQueryBuilder: sortQueryBuilder,
            options: options);

        SortContext<Person> sortContext = new(Array.Empty<SortParameter>(), ["invalid_parameter"]);

        // Act
        var sortQuery = sorter.CreateSortQuery(sortContext);

        // Assert
        Assert.NotNull(sortQuery);
        Assert.Same(defaultSortQuery, sortQuery);
    }

    [Fact]
    public void CreateSortQuery_ShouldReturnDefaultSortQuery_WhenSortContextIsEmpty()
    {
        // Arrange
        ISortQuery<Person> defaultSortQuery = Substitute.For<ISortQuery<Person>>();
        ISortQueryBuilder<Person> sortQueryBuilder = Substitute.For<ISortQueryBuilder<Person>>();
        sortQueryBuilder.IsEmpty.Returns(false);
        sortQueryBuilder.Build().Returns(defaultSortQuery);

        SorterOptions options = CreateOptions(ignoreInvalidParameters: false);

        Sorter<Person> sorter = CreateSorter(
            defaultParameterSortQueryBuilder: sortQueryBuilder,
            options: options);

        // Act
        var sortQuery = sorter.CreateSortQuery(SortContext<Person>.Empty);

        // Assert
        Assert.NotNull(sortQuery);
        Assert.Same(defaultSortQuery, sortQuery);
    }

    [Fact]
    public void CreateSortQuery_ShouldReturnDefaultSortQuery_WhenQueryBuilderIsEmpty()
    {
        // Arrange
        ISortQueryBuilder<Person> queryBuilder = Substitute.For<ISortQueryBuilder<Person>>();
        queryBuilder.IsEmpty.Returns(true);

        ISortQueryBuilderFactory<Person> queryBuilderFactory = Substitute.For<ISortQueryBuilderFactory<Person>>();
        queryBuilderFactory.Create().Returns(queryBuilder);

        ISortQuery<Person> defaultQuery = Substitute.For<ISortQuery<Person>>();
        ISortQueryBuilder<Person> defaultQueryBuilder = Substitute.For<ISortQueryBuilder<Person>>();
        defaultQueryBuilder.IsEmpty.Returns(false);
        defaultQueryBuilder.Build().Returns(defaultQuery);

        SorterOptions options = CreateOptions(parameterNameComparer: StringComparer.OrdinalIgnoreCase);

        Sorter<Person> sorter = CreateSorter(
            sortQueryBuilderFactory: queryBuilderFactory,
            defaultParameterSortQueryBuilder: defaultQueryBuilder,
            options: options);

        SortContext<Person> sortContext = new([new SortParameter(nameof(Person.Name), SortDirection.Ascending)], Array.Empty<string>());

        // Act
        var sortQuery = sorter.CreateSortQuery(sortContext);

        // Assert
        Assert.NotNull(sortQuery);
        Assert.Same(defaultQuery, sortQuery);
    }

    [Fact]
    public void CreateSortQuery_ShouldBuildQuery_WhenSortContextIsValid()
    {
        // Arrange
        ISortQuery<Person> query = Substitute.For<ISortQuery<Person>>();
        ISortQueryBuilder<Person> queryBuilder = Substitute.For<ISortQueryBuilder<Person>>();
        queryBuilder.IsEmpty.Returns(false);
        queryBuilder.Build().Returns(query);

        ISortQueryBuilderFactory<Person> queryBuilderFactory = Substitute.For<ISortQueryBuilderFactory<Person>>();
        queryBuilderFactory.Create().Returns(queryBuilder);

        SorterOptions options = CreateOptions(parameterNameComparer: StringComparer.OrdinalIgnoreCase);

        Sorter<Person> sorter = CreateSorter(
            sortQueryBuilderFactory: queryBuilderFactory,
            options: options);

        SortContext<Person> sortContext = new([new SortParameter(nameof(Person.Name), SortDirection.Ascending)], Array.Empty<string>());
        
        // Act
        var sortQuery = sorter.CreateSortQuery(sortContext);
        
        // Assert
        Assert.NotNull(sortQuery);
        Assert.Same(query, sortQuery);
    }

    [Fact]
    public void Validate_ShouldReturnEmptyContext_WhenNoParametersProvided()
    {
        // Arrange
        var query = "".AsSpan();
        Sorter<Person> sorter = CreateSorter();

        // Act
        var sortContext = sorter.Validate(query);

        // Assert
        Assert.True(sortContext.IsEmpty);
        Assert.True(sortContext.IsValid);
    }

    [Fact]
    public void Validate_ShouldReturnInvalidContext_WhenUnparsableParametersProvided()
    {
        // Arrange
        var query = "-";
        Sorter<Person> sorter = CreateSorter();

        // Act
        var sortContext = sorter.Validate(query.AsSpan());

        // Assert
        Assert.True(sortContext.IsEmpty);
        Assert.False(sortContext.IsValid);
        Assert.Single(sortContext.InvalidParameters);
        Assert.Equal(query, sortContext.InvalidParameters[0]);
    }

    [Fact]
    public void Validate_ShouldReturnInvalidContext_WhenNonExistentParametersProvided()
    {
        // Arrange
        var query = "invalid_parameter";
        Sorter<Person> sorter = CreateSorter();

        // Act
        var sortContext = sorter.Validate(query.AsSpan());

        // Assert
        Assert.True(sortContext.IsEmpty);
        Assert.False(sortContext.IsValid);
        Assert.Single(sortContext.InvalidParameters);
        Assert.Equal(query, sortContext.InvalidParameters[0]);
    }

    [Fact]
    public void Validate_ShouldReturnValidContext_WhenValidParametersProvided()
    {
        // Arrange
        var query = "name,age";
        Sorter<Person> sorter = CreateSorter();

        // Act
        var sortContext = sorter.Validate(query.AsSpan());

        // Assert
        Assert.False(sortContext.IsEmpty);
        Assert.True(sortContext.IsValid);
        Assert.Empty(sortContext.InvalidParameters);
        Assert.Equal(2, sortContext.ValidParameters.Count);
    }
}