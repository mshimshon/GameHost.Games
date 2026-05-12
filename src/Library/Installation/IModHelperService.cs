using GameHost.Games.Lib.Installation.Contracts.Responses.Mods;

namespace GameHost.Games.Lib.Installation;

public interface IModHelperService
{
    Task RemoveCurrentAsync(CancellationToken ct = default);
    Task RemoveByIdAsync(Guid id, CancellationToken ct = default);
    Task<ModListResponse?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ModListResponse?> GetActiveAsync(CancellationToken ct = default);

}
