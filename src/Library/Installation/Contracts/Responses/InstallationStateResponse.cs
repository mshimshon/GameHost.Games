namespace GameHost.Games.Lib.Installation.Contracts.Responses;

public sealed record InstallationStateResponse
{
    public string Id { get; set; } = default!;
    public DateTime InstallDate { get; set; }
}
