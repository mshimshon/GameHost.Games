namespace GameHost.Games.Lib.Installation.Payloads.Responses;

public sealed record VersionResponse
{
    public VersionResponse(string version)
    {
        Version = version;
    }

    public string Version { get; }
}
