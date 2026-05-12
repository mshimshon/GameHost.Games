namespace GameHost.Games.Lib.Installation.Payloads.Responses.GameInfo.StartupParameters;

public sealed class RelatedToResponse
{
    public string Key { get; init; } = default!;
    public string Constraint { get; init; } = default!;
    public string Message { get; init; } = default!;
}
