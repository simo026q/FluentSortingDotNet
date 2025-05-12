using FluentSortingDotNet.Queries;
using FluentSortingDotNet.Testing;

namespace FluentSortingDotNet.Tests.Queries;

public class DefaultSortQueryBuilder_SortQueryTests : SortQueryBuilder_SortQueryTest
{
    protected override ISortQueryBuilder<Person> CreateSortQueryBuilder()
        => new DefaultSortQueryBuilder<Person>();
}