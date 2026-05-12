namespace GameHost.Games.Lib.Installation.Contracts.Responses;

public sealed record ResultResponse
{
    public object? Data { get; set; }
    public ErrorResponse? Error { get; set; }
}
