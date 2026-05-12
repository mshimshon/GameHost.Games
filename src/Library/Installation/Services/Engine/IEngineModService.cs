using GameHost.Games.Lib.Installation.Optionals;

namespace GameHost.Games.Lib.Installation.Services.Engine;

internal interface IEngineModService : IServerModControl
{
    Task RemoveCurrentAsync(CancellationToken ct = default);
}
