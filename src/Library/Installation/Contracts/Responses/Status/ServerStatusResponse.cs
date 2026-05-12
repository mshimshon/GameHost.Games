using GameHost.Games.Lib.Installation.Contracts.Responses.Status.Enums;

namespace GameHost.Games.Lib.Installation.Contracts.Responses.Status;

public sealed record ServerStatusResponse
{
    public ServerStatus Status { get; set; } = ServerStatus.Unknown;
    public ConnectionInfoResponse? ConnectionInfo { get; set; }
}
