namespace GameHost.Games.Lib.Installation.Contracts.Responses;

public sealed record DistroInformationResponse
{
    public string Id { get; set; } = default!;
    public string VersionId { get; set; } = default!;
    public string VersionCodename { get; set; } = default!;

}
