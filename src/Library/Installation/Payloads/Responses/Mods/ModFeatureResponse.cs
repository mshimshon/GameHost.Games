namespace GameHost.Games.Lib.Installation.Payloads.Responses.Mods;

public sealed record ModFeatureResponse
{
    public bool Modding { get; set; }
    public bool ManualModDownload { get; set; }
}
