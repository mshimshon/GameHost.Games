using GameHost.Games.Lib.LinuxGameServerManager.Contracts.Response;
using GameHost.Games.Lib.LinuxGameServerManager.Exceptions;
using LunaticPanel.Core.Utils.Abstraction.Logging;

namespace GameHost.Games.Lib.LinuxGameServerManager.Engine;

internal class LinuxGameServerManagerService : ILinuxGameServerManagerService
{
    private readonly IServerInstallService _serverInstallService;
    private readonly IServerControlService _serverControlService;
    private readonly IServerBackupService _serverBackupService;
    private readonly ICrazyReport<LinuxGameServerManagerService> _crazyReport;

    public LinuxGameServerManagerService(
        IServerInstallService serverInstallService,
        IServerControlService serverControlService,
        IServerBackupService serverBackupService, ICrazyReport<LinuxGameServerManagerService> crazyReport)
    {
        _serverInstallService = serverInstallService;
        _serverControlService = serverControlService;
        _serverBackupService = serverBackupService;
        _crazyReport = crazyReport;
        _crazyReport.SetModule("LGSM_Library");
    }

    public Task BackupAsync(string serverName, CancellationToken ct = default)
        => ProcessWithLock(BaseInfo.INSTALL_LOCK_FILE, () => _serverBackupService.BackupAsync(serverName, ct),
            () => new InstallAlreadyRunningException(), ct);
    public Task GetBackupsAsync(string serverName, CancellationToken ct = default)
                => _serverBackupService.GetBackupsAsync(serverName, ct);
    public Task RestoreBackupAsync(string serverName, string name, CancellationToken ct = default)
            => ProcessWithLock(BaseInfo.INSTALL_LOCK_FILE, () => _serverBackupService.RestoreBackupAsync(serverName, name, ct),
            () => new InstallAlreadyRunningException(), ct);




    public Task ConsoleAsync(string serverName, Func<string, Task> consoleStream, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<IReadOnlyCollection<DetailsResponse>> DetailsAsync(string serverName, bool hideSensitive = false, CancellationToken ct = default)
        => _serverControlService.DetailsAsync(serverName, hideSensitive, ct);
    public Task RestartAsync(string serverName, CancellationToken ct = default)
        => _serverControlService.RestartAsync(serverName, ct);
    public Task StartAsync(string serverName, CancellationToken ct = default)
        => _serverControlService.StartAsync(serverName, ct);

    public Task StopAsync(string serverName, CancellationToken ct = default)
        => _serverControlService.StopAsync(serverName, ct);



    public Task InstallAsync(string serverName, Func<string, CancellationToken, Task> updateProgressStatus, CancellationToken ct = default)
    => ProcessWithLock(BaseInfo.INSTALL_LOCK_FILE, async () =>
    {
        await TrialProcess(() => _serverInstallService.DownloadSoftwareAsync(serverName, ct), ct);
        await TrialProcess(() => _serverInstallService.InstallGameServerAsync(serverName, updateProgressStatus, ct), ct);
    }, () => new InstallAlreadyRunningException(), ct);
    public Task UpdateGameAsync(string serverName, CancellationToken ct = default)
         => ProcessWithLock(BaseInfo.INSTALL_LOCK_FILE, async () =>
         {
             await _serverInstallService.ValidateGameServerAsync(serverName, ct);
             await TrialProcess(() => _serverInstallService.UpdateGameServerAsync(serverName, ct), ct);
         }, () => new UpdateAlreadyRunningException());
    public Task UpdateSoftwareAsync(string serverName, CancellationToken ct = default)
            => ProcessWithLock(BaseInfo.INSTALL_LOCK_FILE, () => _serverInstallService.UpdateSoftwareAsync(serverName, ct),
                () => new InstallAlreadyRunningException(), ct);
    public Task ValidateAsync(string serverName, CancellationToken ct = default)
        => ProcessWithLock(BaseInfo.INSTALL_LOCK_FILE, () => _serverInstallService.ValidateGameServerAsync(serverName, ct),
            () => new InstallAlreadyRunningException(), ct);

    private async Task ProcessWithLock(string lockFile, Func<Task> action, Func<Exception> onLockFailed, CancellationToken ct = default)
    {
        FileStream? lockHandle = default;
        try
        {
            lockHandle = File.Open(lockFile, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None);
            await action();
        }
        catch
        {
            throw;
        }
        finally
        {
            if (lockHandle != default)
                try
                {
                    lockHandle.Close();
                    File.Delete(lockFile);
                }
                catch { }
        }
    }
    private async Task TrialProcess(Func<Task> action, CancellationToken ct = default)
    {
        int maxTry = 2;
        do
        {
            if (ct.IsCancellationRequested) return;
            Exception? failure = default;
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                failure = ex;
            }
            bool hasFailed = failure != default;
            bool hasSucceeded = failure == default;
            if (ct.IsCancellationRequested)
            {
                if (hasFailed)
                    throw failure!;
                return;
            }
            if (hasFailed && maxTry > 0)
            {
                _crazyReport.ReportErrorException(failure!.Message, failure);
                _crazyReport.ReportInfo("Trial Retrying Now");

                maxTry--;
                continue;
            }
            else if (hasSucceeded)
            {
                _crazyReport.ReportSuccess("Trial Operation Successful");
                break;
            }
            else
            {
                _crazyReport.ReportErrorException(failure!.Message, failure);
                _crazyReport.ReportError("Trial Stopped No More Try");
                throw failure!;
            }

        } while (maxTry >= 0 || ct.IsCancellationRequested);
    }

    public Task<IEnumerable<string>> CheckUpdateGameAsync(string serverName, CancellationToken ct = default)
        => _serverInstallService.CheckUpdateGameServerAsync(serverName, ct);
}
