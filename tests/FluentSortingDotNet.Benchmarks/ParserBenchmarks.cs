using BenchmarkDotNet.Attributes;
using FluentSortingDotNet;
using FluentSortingDotNet.Parser;

[MemoryDiagnoser(false)]
public class ParserBenchmarks
{
    [Params("a", "-a,b", "a,b,-c,d,-e,-f,g")]
    public string Query { get; set; }

    private readonly DefaultSortParameterParser DefaultParser = DefaultSortParameterParser.Instance;

    [Benchmark(Baseline = true)]
    public SortParameter ParseFirst()
    {
        ReadOnlySpan<char> query = Query.AsSpan();

        if (DefaultParser.TryGetNextParameter(ref query, out var parameter) && DefaultParser.TryParseParameter(parameter, out var sortParameter))
        {
            return sortParameter;
        }

        return SortParameter.Empty;
    }
}
