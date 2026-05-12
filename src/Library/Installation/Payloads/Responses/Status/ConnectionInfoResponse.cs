namespace GameHost.Games.Lib.Installation.Payloads.Responses.Status;

public record ConnectionInfoResponse
{

    public string Address { get; set; } = default!;
    public List<PortInfoResponse> PortInfoResponses { get; set; } = new();
}
