namespace GameHost.Games.Lib.Installation.Contracts.Responses;

public sealed record ServerUpdateResponse
{
    public VersionResponse? UpdateToVersion { get; set; }
    public VersionResponse? CurrentVersion { get; set; }
}
