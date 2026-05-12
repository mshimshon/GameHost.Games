using GameHost.Games.Lib.Installation.Payloads.Responses.Status.Enums;

namespace GameHost.Games.Lib.Installation.Payloads.Responses.Status;

public sealed record ServerStatusResponse
{
    public ServerStatus Status { get; set; } = ServerStatus.Unknown;
    public ConnectionInfoResponse? ConnectionInfo { get; set; }
}
