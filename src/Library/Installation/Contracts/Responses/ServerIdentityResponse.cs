namespace GameHost.Games.Lib.Installation.Contracts.Responses;

public sealed record ServerIdentityResponse
{
    public string Id { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
}
