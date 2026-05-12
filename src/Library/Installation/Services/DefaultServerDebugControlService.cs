using GameHost.Games.Lib.Installation.Optionals;

namespace GameHost.Games.Lib.Installation.Services;

internal class DefaultServerDebugControlService : IServerDebugControl
{
    public Task<IEnumerable<string>> GetLogFiles(CancellationToken ct = default) => throw new NotImplementedException();
    public Task<bool> HasDebugSupportAsync(CancellationToken ct = default) => Task.FromResult(false);
}
