using FluentSortingDotNet.Queries;
using FluentSortingDotNet.Testing;

namespace FluentSortingDotNet.Tests.Queries;

public class ExpressionSortQueryBuilderTests : SortQueryBuilderTests
{
    protected override ISortQueryBuilder<Person> CreateSortQueryBuilder()
        => new ExpressionSortQueryBuilder<Person>();
}