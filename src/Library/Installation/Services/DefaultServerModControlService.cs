using GameHost.Games.Lib.Installation.Contracts.Responses.Mods;
using GameHost.Games.Lib.Installation.Exceptions;
using GameHost.Games.Lib.Installation.Optionals;

namespace GameHost.Games.Lib.Installation.Services;

internal class DefaultServerModControlService : IServerModControl
{
    public Task CleanModListAsync(CancellationToken ct = default) => throw new ModServiceUnavailableException();
    public Task<ModFeatureResponse> GetDetailsAsync(CancellationToken ct = default) => throw new NotImplementedException();
    public Task<bool> IsSupportedAsync(CancellationToken ct = default)
        => Task.FromResult(false);
    public Task ProcessModListAsync(CancellationToken ct = default) => throw new ModServiceUnavailableException();
}
