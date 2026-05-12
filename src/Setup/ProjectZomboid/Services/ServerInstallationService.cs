using GameHost.Games.Lib.Installation;
using GameHost.Games.Lib.Installation.Payloads.Responses;
using GameHost.Games.Lib.LinuxGameServerManager;
using LunaticPanel.Core.Utils.Abstraction.Plugin.Location;

namespace GameHost.Games.ProjectZomboid.Console.Services;

internal class ServerInstallationService : IServerInstallation
{
    private readonly ILinuxGameServerManagerService _linuxGameServerManagerService;
    private readonly IDistroDependencyFileService _distroDependencyFileService;
    private readonly IPluginSystemLocation _pluginSystemLocation;
    private readonly IPluginUserLocation _pluginUserLocation;
    public ServerInstallationService(ILinuxGameServerManagerService linuxGameServerManagerService, IDistroDependencyFileService distroDependencyFileService, IPluginLocation pluginLocation)
    {
        _linuxGameServerManagerService = linuxGameServerManagerService;
        _distroDependencyFileService = distroDependencyFileService;
        _pluginSystemLocation = pluginLocation;
        _pluginUserLocation = pluginLocation;
        _pluginUserLocation.SetUsername(BaseInfo.USERNAME);
    }

    public async Task<ServerUpdateResponse?> CheckUpdateAsync(CancellationToken ct = default)
    {
        var result = await _linuxGameServerManagerService.CheckUpdateGameAsync(BaseInfo.LGSM_SERVER_ID, ct);
        string localExtracted = "", remoteExtracted = "";
        bool isDone = false;
        foreach (var item in result)
        {
            bool localFound = item.Contains("* Local build:", StringComparison.OrdinalIgnoreCase);
            bool remoteFound = item.Contains("* Remote build:", StringComparison.OrdinalIgnoreCase);
            if (localFound)
            {
                string[] splitLocal = item.Split(':');
                if (splitLocal.Length < 2) continue;
                localExtracted = splitLocal[1];
            }
            if (remoteFound)
            {
                string[] splitRemote = item.Split(':');
                if (splitRemote.Length < 2) continue;
                remoteExtracted = splitRemote[1].Trim();
            }

            isDone = !string.IsNullOrWhiteSpace(remoteExtracted) && !string.IsNullOrWhiteSpace(localExtracted);
            if (isDone) break;
        }
        if (!isDone) return default;
        return new ServerUpdateResponse()
        {
            CurrentVersion = new(localExtracted),
            UpdateToVersion = new(remoteExtracted)
        };
    }

    public Task<ServerIdentityResponse> GetServerIdentityAsync(CancellationToken ct = default)
        => Task.FromResult(new ServerIdentityResponse()
        {
            DisplayName = BaseInfo.SERVER_NAME,
            Id = BaseInfo.SERVER_ID
        });

    public async Task<VersionResponse?> GetVersionAsync(CancellationToken ct = default)
    {
        var result = await CheckUpdateAsync(ct);
        return result?.CurrentVersion;
    }

    public Task InitializeAsync(CancellationToken ct = default) => Task.CompletedTask;

    public async Task InstallAsync(Func<string, CancellationToken, Task> updateProgressStatus, CancellationToken ct = default)
    {
        await _distroDependencyFileService.DownloadOfficialDistroDependencyFile(ct);
        await _distroDependencyFileService.InstallDependenciesAsync(BaseInfo.SERVER_ID, updateProgressStatus, ct);
        await _linuxGameServerManagerService.InstallAsync(BaseInfo.LGSM_SERVER_ID, updateProgressStatus, ct);
    }

    public Task PostInstallAsync(CancellationToken ct = default) => Task.CompletedTask;
    public async Task UpdateAsync(CancellationToken ct = default)
    {
        await _linuxGameServerManagerService.UpdateSoftwareAsync(BaseInfo.LGSM_SERVER_ID, ct);
        await _linuxGameServerManagerService.UpdateGameAsync(BaseInfo.USERNAME, ct);
    }
}
