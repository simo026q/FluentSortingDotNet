using BenchmarkDotNet.Running;

var targets = GetBenchmarkTargets(args);

if (targets.HasFlag(BenchmarkTargets.Parser))
{
    BenchmarkRunner.Run<ParserBenchmarks>();
}

if (targets.HasFlag(BenchmarkTargets.QueryBuilder))
{
    BenchmarkRunner.Run<QueryBuilderBenchmarks>();
}

static BenchmarkTargets GetBenchmarkTargets(string[] args)
{
    if (args.Length == 0)
    {
        return BenchmarkTargets.All;
    }

    var targets = BenchmarkTargets.None;

    foreach (var arg in args)
    {
        if (Enum.TryParse<BenchmarkTargets>(arg, ignoreCase: true, out var target))
        {
            targets |= target;
        }
    }

    return targets;
}

[Flags]
public enum BenchmarkTargets
{
    None = 0,
    Parser = 1,
    QueryBuilder = 2,
    All = Parser | QueryBuilder
}