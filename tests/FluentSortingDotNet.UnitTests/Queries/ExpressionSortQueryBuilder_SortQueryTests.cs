using FluentSortingDotNet.Queries;
using FluentSortingDotNet.Testing;

namespace FluentSortingDotNet.Tests.Queries;

public class ExpressionSortQueryBuilder_SortQueryTests : SortQueryBuilder_SortQueryTest
{
    protected override ISortQueryBuilder<Person> CreateSortQueryBuilder()
        => new ExpressionSortQueryBuilder<Person>();
}