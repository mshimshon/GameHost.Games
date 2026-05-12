namespace GameHost.Games.Lib.Installation.Payloads.Responses;

public sealed record DistroDependencyFileResponse
{
    public List<string> Common { get; set; } = default!;
    public Dictionary<string, string[]>? Specific { get; set; }
}
