using FluentSortingDotNet.Queries;
using FluentSortingDotNet.Testing;

namespace FluentSortingDotNet.Tests.Queries;

public class DefaultSortQueryBuilderTests : SortQueryBuilderTests
{
    protected override ISortQueryBuilder<Person> CreateSortQueryBuilder()
        => new DefaultSortQueryBuilder<Person>();
}