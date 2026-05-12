namespace GameHost.Games.Lib.Installation.Payloads.Responses;

public sealed record DistroInformationResponse
{
    public string Id { get; set; } = default!;
    public string VersionId { get; set; } = default!;
    public string VersionCodename { get; set; } = default!;

}
