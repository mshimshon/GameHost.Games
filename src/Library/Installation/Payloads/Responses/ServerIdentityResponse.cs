namespace GameHost.Games.Lib.Installation.Payloads.Responses;

public sealed record ServerIdentityResponse
{
    public string Id { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
}
