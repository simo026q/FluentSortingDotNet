using FluentSortingDotNet.Parsers;
using FluentSortingDotNet.Queries;

namespace FluentSortingDotNet.Testing;

public sealed class PersonSorter(ISortParameterParser? parser = null, ISortQueryBuilderFactory<Person>? sortQueryBuilderBuilder = null, ISortQueryBuilder<Person>? defaultParameterSortQueryBuilder = null) 
    : Sorter<Person>(GetParser(parser), GetSortQueryBuilderFactory(sortQueryBuilderBuilder), GetDefaultParameterSortQueryBuilder(defaultParameterSortQueryBuilder))
{
    protected override void Configure(SortBuilder<Person> builder)
    {
        builder.ForParameter(p => p.Name, "name").Default(SortDirection.Descending);
        builder.ForParameter(p => p.Age, "age");
    }

    private static ISortParameterParser GetParser(ISortParameterParser? parser) => parser ?? DefaultSortParameterParser.Instance;
    private static ISortQueryBuilderFactory<Person> GetSortQueryBuilderFactory(ISortQueryBuilderFactory<Person>? sortQueryBuilderFactory) => sortQueryBuilderFactory ?? DefaultSortQueryBuilderFactory<Person>.Instance;
    private static ISortQueryBuilder<Person> GetDefaultParameterSortQueryBuilder(ISortQueryBuilder<Person>? defaultParameterSortQueryBuilder) => defaultParameterSortQueryBuilder ?? new ExpressionSortQueryBuilder<Person>();
}