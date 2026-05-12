namespace GameHost.Games.Lib.Installation.Payloads.Responses.Mods;

public sealed class ModResponse
{
    public string Id { get; set; } = default!;
    public string? Name { get; set; }
    public List<ModResponse>? Dependencies { get; set; }
}
