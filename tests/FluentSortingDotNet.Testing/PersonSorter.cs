using FluentSortingDotNet.Parsers;
using FluentSortingDotNet.Queries;

namespace FluentSortingDotNet.Testing;

public sealed class PersonSorter(
    ISortParameterParser? parser = null,
    ISortQueryBuilderFactory<Person>? sortQueryBuilderFactory = null,
    ISortQueryBuilder<Person>? defaultParameterSortQueryBuilder = null,
    SorterOptions? options = null)
    : Sorter<Person>(
        parser ?? DefaultSortParameterParser.Instance,
        sortQueryBuilderFactory ?? DefaultSortQueryBuilderFactory<Person>.Instance,
        defaultParameterSortQueryBuilder ?? new ExpressionSortQueryBuilder<Person>(),
        options)
{
    protected override void Configure(SortBuilder<Person> builder)
    {
        builder.ForParameter(p => p.Name).IsDefault(SortDirection.Descending);
        builder.ForParameter(p => p.DateOfBirth, "Age").ReverseDirection();
    }
}