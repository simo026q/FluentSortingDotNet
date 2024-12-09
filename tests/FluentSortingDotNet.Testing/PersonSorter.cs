using FluentSortingDotNet.Parser;
using FluentSortingDotNet.Queries;

namespace FluentSortingDotNet.Testing;

public sealed class PersonSorter(ISortParameterParser? parser = null, ISortQueryBuilder<Person>? sortQueryBuilder = null, ISortQueryBuilder<Person>? defaultParameterSortQueryBuilder = null) 
    : Sorter<Person>(GetParser(parser), GetSortQueryBuilder(sortQueryBuilder), GetDefaultParameterSortQueryBuilder(defaultParameterSortQueryBuilder))
{
    protected override void Configure(SortBuilder<Person> builder)
    {
        builder.ForParameter(p => p.Name, "name").Default(SortDirection.Descending);
        builder.ForParameter(p => p.Age, "age");
    }

    private static ISortParameterParser GetParser(ISortParameterParser? parser) => parser ?? DefaultSortParameterParser.Instance;
    private static ISortQueryBuilder<Person> GetSortQueryBuilder(ISortQueryBuilder<Person>? sortQueryBuilder) => sortQueryBuilder ?? DefaultSortQueryBuilder<Person>.Instance;
    private static ISortQueryBuilder<Person> GetDefaultParameterSortQueryBuilder(ISortQueryBuilder<Person>? defaultParameterSortQueryBuilder) => defaultParameterSortQueryBuilder ?? ExpressionSortQueryBuilder<Person>.Instance;
}