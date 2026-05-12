namespace GameHost.Games.Lib.Installation.Contracts.Responses.Status;


public record PortInfoResponse
{
    public string Name { get; set; } = default!;
    public string Port { get; set; } = default!;
    public string Protocol { get; set; } = default!;
}
