using GameHost.Games.Lib.Installation.Contracts.Responses;
using GameHost.Games.Lib.Installation.Contracts.Responses.GameInfo;

namespace GameHost.Games.Lib.Installation;

public interface IMetadataService
{
    Task<ManifestResponse> GetManifestAsync(CancellationToken ct = default);

    Task<GameInfoResponse> GetGameInfoAsync(CancellationToken ct = default);
}
