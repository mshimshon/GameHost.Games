using GameHost.Games.Lib.Installation.Contracts.Responses;

namespace GameHost.Games.Lib.Installation;

public interface IServerInstallation
{
    Task InstallAsync(Func<string, CancellationToken, Task> updateProgressStatus, CancellationToken ct = default);
    Task<ServerUpdateResponse?> CheckUpdateAsync(CancellationToken ct = default);
    Task UpdateAsync(CancellationToken ct = default);
    Task<VersionResponse?> GetVersionAsync(CancellationToken ct = default);
    Task InitializeAsync(CancellationToken ct = default);
    Task PostInstallAsync(CancellationToken ct = default);
}
