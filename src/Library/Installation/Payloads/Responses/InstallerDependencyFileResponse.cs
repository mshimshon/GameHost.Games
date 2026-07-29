namespace GameHost.Games.Lib.Installation.Payloads.Responses;

internal sealed record InstallerDependencyFileResponse
{
    public string OperatingSystem { get; init; } = default!;
    public string OperatingSystemVersion { get; init; } = default!;
    public List<string> Dependencies { get; init; } = new List<string>();
}
