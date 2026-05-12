namespace GameHost.Games.Lib.Installation.Contracts.Responses;

public sealed class ManifestResponse
{
    public string Id { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string? Icon { get; set; }
    public List<string> DistroCompatibility { get; set; } = default!;
    public string InstallerName { get; set; } = default!;
    public string InstallerSource { get; set; } = default!;
}
