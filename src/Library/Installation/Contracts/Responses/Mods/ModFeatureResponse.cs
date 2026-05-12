namespace GameHost.Games.Lib.Installation.Contracts.Responses.Mods;

public sealed record ModFeatureResponse
{
    public bool RequiredManualDownload { get; set; }
}
