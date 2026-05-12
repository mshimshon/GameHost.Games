namespace GameHost.Games.Lib.Installation.Payloads.Responses.Mods;

public sealed record ModListResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public Dictionary<string, List<ModResponse>> Mods { get; set; } = new();
}
