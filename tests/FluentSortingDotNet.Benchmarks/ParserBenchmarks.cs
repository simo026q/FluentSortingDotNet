using BenchmarkDotNet.Attributes;
using FluentSortingDotNet;
using FluentSortingDotNet.Parsers;

[MemoryDiagnoser(false), MarkdownExporter]
public class ParserBenchmarks
{
    [Params("a,-b", "-a,b", "a,b,-c,d,-e,-f,g")]
    public string Query { get; set; } = null!;

    private static readonly DefaultSortParameterParser DefaultParser = DefaultSortParameterParser.Instance;

    [Benchmark]
    public SortParameter ParseFirst()
    {
        ReadOnlySpan<char> query = Query.AsSpan();

        if (DefaultParser.TryGetNextParameter(ref query, out var parameter) && DefaultParser.TryParseParameter(parameter, out var sortParameter))
        {
            return sortParameter;
        }

        throw new InvalidOperationException("Failed to parse the first sort parameter.");
    }
}
