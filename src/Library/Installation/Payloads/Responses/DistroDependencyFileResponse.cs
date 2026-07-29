namespace GameHost.Games.Lib.Installation.Payloads.Responses;

public sealed record DistroDependencyFileResponse
{
    public List<string> Common { get; init; } = default!;
    public Dictionary<string, string[]>? Specific { get; init; }
}
