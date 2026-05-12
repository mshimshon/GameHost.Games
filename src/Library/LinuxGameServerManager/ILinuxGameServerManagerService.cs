using GameHost.Games.Lib.LinuxGameServerManager.Contracts.Response;

namespace GameHost.Games.Lib.LinuxGameServerManager;

public interface ILinuxGameServerManagerService
{
    Task BackupAsync(string serverName, CancellationToken ct = default);
    Task GetBackupsAsync(string serverName, CancellationToken ct = default);
    Task RestoreBackupAsync(string serverName, string name, CancellationToken ct = default);
    Task ConsoleAsync(string serverName, Func<string, Task> consoleStream, CancellationToken ct = default);
    Task<IReadOnlyCollection<DetailsResponse>> DetailsAsync(string serverName, bool hideSensitive = false, CancellationToken ct = default);
    Task StopAsync(string serverName, CancellationToken ct = default);
    Task StartAsync(string serverName, CancellationToken ct = default);
    Task RestartAsync(string serverName, CancellationToken ct = default);
    Task InstallAsync(string serverName, Func<string, CancellationToken, Task> updateProgressStatus, CancellationToken ct = default);
    Task ValidateAsync(string serverName, CancellationToken ct = default);
    Task UpdateGameAsync(string serverName, CancellationToken ct = default);
    Task<IEnumerable<string>> CheckUpdateGameAsync(string serverName, CancellationToken ct = default);
    Task UpdateSoftwareAsync(string serverName, CancellationToken ct = default);
}
