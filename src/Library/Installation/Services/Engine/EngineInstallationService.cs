using GameHost.Core.Features;
using GameHost.Games.Lib.Installation.Payloads.Responses;
using GameHost.Games.Lib.Installation.Exceptions;
using LunaticPanel.Core.Utils.Abstraction.Plugin.Location;
using LunaticPanel.Core.Utils.Abstraction.SafeFileWriter;
using System.Text.Json;

namespace GameHost.Games.Lib.Installation.Services.Engine;

internal class EngineInstallationService : IEngineInstallation
{
    private readonly IServerInstallation _serverInstallation;
    private readonly ISafeFileWriter _safeFileWriter;
    private readonly IPluginSystemLocation _pluginSystemLocation;
    private readonly IEngineInitializerService _engineInitializerService;
    private readonly IMetadataService _metadataService;
    private readonly string _installProgressFile;

    public EngineInstallationService(IServerInstallation serverInstallation,
        ISafeFileWriter safeFileWriter,
        IPluginLocation pluginLocation,
        IEngineInitializerService engineInitializerService,
        IMetadataService metadataService)
    {
        _safeFileWriter = safeFileWriter;
        _serverInstallation = serverInstallation;
        _pluginSystemLocation = pluginLocation;
        _engineInitializerService = engineInitializerService;
        _metadataService = metadataService;
        _installProgressFile = _pluginSystemLocation.GetConfigFor(LinuxGameServerKeys.MODULE_NAME, LinuxGameServerKeys.SERVER_INSTALL_PROGRESS_FILE);
    }


    private async Task WriteProgressFile(InstallationProgressResponse next, CancellationToken ct = default)
    {
        string toWriteJson = JsonSerializer.Serialize(next, BaseInfo.jsonSerializerOptions);
        if (ct.IsCancellationRequested) return;
        await _safeFileWriter.WriteThenCopyFileAsync(_installProgressFile, toWriteJson);
    }


    public async Task InstallAsync(Func<string, CancellationToken, Task> updateProgressStatus, CancellationToken ct = default)
    {
        object _lock = new();
        string installStateFile = _pluginSystemLocation.GetConfigFor(LinuxGameServerKeys.MODULE_NAME, LinuxGameServerKeys.SERVER_INSTALL_STATE_FILE);
        if (File.Exists(installStateFile))
            throw new ServerAlreadyInstalledException();
        _pluginSystemLocation.GetConfigFor(LinuxGameServerKeys.MODULE_NAME, LinuxGameServerKeys.SERVER_INSTALL_PROGRESS_FILE);
        var manifest = await _metadataService.GetManifestAsync(ct);
        InstallationProgressResponse? nextProgressStatus = default;

        var progress = new InstallationProgressResponse()
        {
            CurrentStep = $"Installing {manifest.DisplayName}...", // TODO: LOCALIZe
            DisplayName = manifest.DisplayName,
            Id = manifest.Id,
            IsInstalling = true
        };
        bool isProgressReportBusy = false;
        var drainCts = new CancellationTokenSource();
        var writeProgress = async () =>
        {
            lock (_lock)
            {
                if (isProgressReportBusy) return;
                isProgressReportBusy = true;
            }

            do
            {
                if (nextProgressStatus == default) break;
                var next = nextProgressStatus;
                nextProgressStatus = default;
                await WriteProgressFile(next, drainCts.Token);
                await Task.Delay(50);
            } while (nextProgressStatus != default && !drainCts.IsCancellationRequested);
            lock (_lock)
                isProgressReportBusy = false;
        };
        try
        {
            string json = JsonSerializer.Serialize(progress, BaseInfo.jsonSerializerOptions);
            await _serverInstallation.InstallAsync(async (status, token) =>
            {
                lock (_lock)
                {
                    nextProgressStatus = progress with
                    {
                        CurrentStep = status
                    };
                    if (isProgressReportBusy) return;
                }
                _ = writeProgress();

            }, ct);
            drainCts.Cancel();
            var installState = new InstallationStateResponse()
            {
                Id = manifest.Id,
                InstallDate = DateTime.UtcNow
            };
            string installStateJson = JsonSerializer.Serialize(installState, BaseInfo.jsonSerializerOptions);
            await _safeFileWriter.WriteThenCopyFileAsync(installStateFile, installStateJson);
            File.Delete(_installProgressFile);
            await PostInstallAsync();

        }
        catch
        {
            drainCts.Cancel();
            progress = progress with
            {
                CurrentStep = $"Failed to Install {manifest.DisplayName}", // TODO: LOCALIZe
                IsInstalling = false,
                FailureReason = "Failed to install the game server check log." // TODO: LOCALIZe
            };
            string json = JsonSerializer.Serialize(progress, BaseInfo.jsonSerializerOptions);
            await _safeFileWriter.WriteThenCopyFileAsync(_installProgressFile, json);

            throw;
        }

    }
    public Task<ServerUpdateResponse?> CheckUpdateAsync(CancellationToken ct = default) => _serverInstallation.CheckUpdateAsync(ct);
    public Task UpdateAsync(CancellationToken ct = default) => _serverInstallation.UpdateAsync(ct);
    public Task<VersionResponse?> GetVersionAsync(CancellationToken ct = default) => _serverInstallation.GetVersionAsync(ct);
    public async Task InitializeAsync(CancellationToken ct = default)
    {
        await _engineInitializerService.InitializeAsync(ct);
        await _serverInstallation.InitializeAsync(ct);

    }

    public async Task PostInstallAsync(CancellationToken ct = default)
    {

        // ServerIdentityResponse
        await _serverInstallation.PostInstallAsync(ct);
    }

}
