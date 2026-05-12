namespace GameHost.Games.Lib.Installation.Contracts.Responses;

public sealed record InstallationProgressResponse
{
    public string? FailureReason { get; set; }
    public bool IsInstalling { get; set; }
    public string CurrentStep { get; set; } = default!;
    public string Id { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
}
