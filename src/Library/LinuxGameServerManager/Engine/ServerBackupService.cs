namespace GameHost.Games.Lib.LinuxGameServerManager.Engine;

internal class ServerBackupService : IServerBackupService
{
    public Task BackupAsync(string serverName, CancellationToken ct = default) => throw new NotImplementedException();
    public Task GetBackupsAsync(string serverName, CancellationToken ct = default) => throw new NotImplementedException();
    public Task RestoreBackupAsync(string serverName, string name, CancellationToken ct = default) => throw new NotImplementedException();
}
