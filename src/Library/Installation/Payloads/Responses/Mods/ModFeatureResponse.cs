namespace GameHost.Games.Lib.Installation.Payloads.Responses.Mods;

public sealed record ModFeatureResponse
{
    public bool RequiredManualDownload { get; set; }
}
