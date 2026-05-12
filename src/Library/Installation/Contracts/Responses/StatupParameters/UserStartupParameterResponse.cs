namespace GameHost.Games.Lib.Installation.Contracts.Responses.StatupParameters;

public sealed record UserStartupParameterResponse
{
    public string Key { get; set; } = default!;
    public string Value { get; set; } = default!;
}
