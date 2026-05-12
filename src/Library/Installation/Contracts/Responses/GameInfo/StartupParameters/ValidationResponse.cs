namespace GameHost.Games.Lib.Installation.Contracts.Responses.GameInfo.StartupParameters;

public sealed class ValidationResponse
{
    public int? Min { get; init; }
    public int? Max { get; init; }
    public int? MinLength { get; init; }
    public int? MaxLength { get; init; }
    public string? Pattern { get; init; }
    public List<AllowedValueResponse>? AllowedValues { get; init; }
    public string? UnitPrefix { get; init; }
    public string? UnitSuffix { get; init; }
}
