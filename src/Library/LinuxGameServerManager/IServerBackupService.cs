namespace GameHost.Games.Lib.LinuxGameServerManager;

public interface IServerBackupService
{
    Task BackupAsync(string serverName, CancellationToken ct = default);
    Task GetBackupsAsync(string serverName, CancellationToken ct = default);
    Task RestoreBackupAsync(string serverName, string name, CancellationToken ct = default);
}
