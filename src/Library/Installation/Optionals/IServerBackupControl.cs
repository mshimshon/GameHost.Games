namespace GameHost.Games.Lib.Installation.Optionals;

public interface IServerBackupControl
{
    Task<bool> HasBackupSupportAsync(CancellationToken ct = default);
    Task BackupAsync(CancellationToken ct = default);
    Task RestoreBackupAsync(string name, CancellationToken ct = default);
}
