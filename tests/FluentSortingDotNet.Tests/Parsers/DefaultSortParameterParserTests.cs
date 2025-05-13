using FluentSortingDotNet.Parsers;

namespace FluentSortingDotNet.Tests.Parsers;

public class DefaultSortParameterParserTests
{
    [Fact]
    public void TryGetNextParameter_WithValidSortParameter_ReturnsTrueAndOutsCorrectParameterName()
    {
        // Arrange
        var sortParameter = "name".AsSpan();

        // Act
        var result = DefaultSortParameterParser.Instance.TryGetNextParameter(ref sortParameter, out ReadOnlySpan<char> parameter);

        // Assert
        Assert.True(result);
        Assert.Equal("name", parameter);
    }

    [Fact]
    public void TryGetNextParameter_WithMultipleValidSortParameters_ReturnsTrueAndOutsCorrectParameterName()
    {
        // Arrange
        var sortParameter = "name,age".AsSpan();

        // Act
        var result1 = DefaultSortParameterParser.Instance.TryGetNextParameter(ref sortParameter, out ReadOnlySpan<char> parameter1);
        var result2 = DefaultSortParameterParser.Instance.TryGetNextParameter(ref sortParameter, out ReadOnlySpan<char> parameter2);

        // Assert
        Assert.True(result1);
        Assert.Equal("name", parameter1);
        Assert.True(result2);
        Assert.Equal("age", parameter2);
    }

    [Fact]
    public void TryGetNextParameter_WithEmptySortParameter_ReturnsFalseAndOutsEmptySpan()
    {
        // Arrange
        var sortParameter = ReadOnlySpan<char>.Empty;

        // Act
        var result = DefaultSortParameterParser.Instance.TryGetNextParameter(ref sortParameter, out ReadOnlySpan<char> parameter);

        // Assert
        Assert.False(result);
        Assert.True(parameter.IsEmpty);
    }

    [Fact]
    public void TryParseParameter_WithValidAscendingSortParameter_ReturnsTrueAndOutsCorrectSortParameter()
    {
        // Arrange
        var parameter = "name".AsSpan();

        // Act
        var result = DefaultSortParameterParser.Instance.TryParseParameter(parameter, out SortParameter sortParameter);

        // Assert
        Assert.True(result);
        Assert.Equal("name", sortParameter.Name);
        Assert.Equal(SortDirection.Ascending, sortParameter.Direction);
    }

    [Fact]
    public void TryParseParameter_WithValidDescendingSortParameter_ReturnsTrueAndOutsCorrectSortParameter()
    {
        // Arrange
        var parameter = "-name".AsSpan();

        // Act
        var result = DefaultSortParameterParser.Instance.TryParseParameter(parameter, out SortParameter sortParameter);

        // Assert
        Assert.True(result);
        Assert.Equal("name", sortParameter.Name);
        Assert.Equal(SortDirection.Descending, sortParameter.Direction);
    }

    [Fact]
    public void TryParseParameter_WithEmptySortParameter_ReturnsFalseAndOutsEmptySortParameter()
    {
        // Arrange
        var parameter = ReadOnlySpan<char>.Empty;

        // Act
        var result = DefaultSortParameterParser.Instance.TryParseParameter(parameter, out SortParameter sortParameter);

        // Assert
        Assert.False(result);
        Assert.Equal(string.Empty, sortParameter.Name);
        Assert.Equal(SortDirection.Ascending, sortParameter.Direction);
    }

    [Fact]
    public void TryParseParameter_WithEmptySortParameterAfterMinus_ReturnsFalseAndOutsEmptySortParameter()
    {
        // Arrange
        var parameter = "-".AsSpan();

        // Act
        var result = DefaultSortParameterParser.Instance.TryParseParameter(parameter, out SortParameter sortParameter);

        // Assert
        Assert.False(result);
        Assert.Equal(string.Empty, sortParameter.Name);
        Assert.Equal(SortDirection.Ascending, sortParameter.Direction);
    }
}
