using GameHost.Games.Lib.Installation.Optionals;

namespace GameHost.Games.Lib.Installation.Services;

internal class DefaultServerBackupService : IServerBackupControl
{
    public Task BackupAsync(CancellationToken ct = default) => throw new NotImplementedException();
    public Task<bool> HasBackupSupportAsync(CancellationToken ct = default)
         => Task.FromResult(false);
    public Task RestoreBackupAsync(string name, CancellationToken ct = default) => throw new NotImplementedException();
}
