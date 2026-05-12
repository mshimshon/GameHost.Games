namespace GameHost.Games.Lib.Installation.Payloads.Responses;

public sealed record ResultResponse
{
    public object? Data { get; set; }
    public ErrorResponse? Error { get; set; }
}
