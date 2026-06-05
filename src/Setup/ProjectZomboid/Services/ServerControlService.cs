using GameHost.Games.Lib.Installation;
using GameHost.Games.Lib.Installation.Optionals;
using GameHost.Games.Lib.Installation.Payloads.Responses.Status;
using GameHost.Games.Lib.LinuxGameServerManager;
using GameHost.Games.ProjectZomboid.Console.Extensions;
using LinuxGameServerManager.Extension;
using LunaticPanel.Core.Utils.Abstraction.LinuxCommand;
using LunaticPanel.Core.Utils.Abstraction.SafeFileWriter;

namespace GameHost.Games.ProjectZomboid.Console.Services;

internal class ServerControlService : IServerControl
{
    private readonly ILinuxGameServerManagerService _linuxGameServerManagerService;
    private readonly IStartParamsHelper _startParamsHelper;
    private readonly IServerModControl _serverModControl;
    private readonly ILinuxCommand _linuxCommand;
    private readonly ISafeFileWriter _safeFileWriter;

    public ServerControlService(ILinuxGameServerManagerService linuxGameServerManagerService,
        IStartParamsHelper startParamsHelper,
        IServerModControl serverModControl, ILinuxCommand linuxCommand, ISafeFileWriter safeFileWriter)
    {
        _linuxGameServerManagerService = linuxGameServerManagerService;
        _startParamsHelper = startParamsHelper;
        _serverModControl = serverModControl;
        _linuxCommand = linuxCommand;
        _safeFileWriter = safeFileWriter;
    }
    public async Task ConsoleAsync(Func<string, Task> consoleStream, CancellationToken ct = default)
        => await _linuxGameServerManagerService.ConsoleAsync(BaseInfo.LGSM_SERVER_ID, consoleStream, ct);

    public async Task RestartAsync(CancellationToken ct = default)
    {
        await EnsureConfigCreated(ct);
        await ApplyStartupParameters(ct);
        await _serverModControl.ProcessModListAsync(ct);
        await _linuxGameServerManagerService.RestartAsync(BaseInfo.LGSM_SERVER_ID, ct);
    }
    public async Task StartAsync(CancellationToken ct = default)
    {
        await EnsureConfigCreated(ct);
        await ApplyStartupParameters(ct);
        await _serverModControl.ProcessModListAsync(ct);
        //await _linuxGameServerManagerService.StartAsync(BaseInfo.LGSM_SERVER_ID, ct);
    }

    public async Task<ServerStatusResponse> StatusAsync(CancellationToken ct = default)
    {
        var details = await _linuxGameServerManagerService.DetailsAsync(BaseInfo.LGSM_SERVER_ID, false, ct);
        return details.ConvertToServerStatus();
    }
    public async Task StopAsync(CancellationToken ct = default)
        => await _linuxGameServerManagerService.StopAsync(BaseInfo.LGSM_SERVER_ID, ct);

    private async Task ApplyStartupParameters(CancellationToken ct = default)
    {
        var startup = await _startParamsHelper.GetStartupParametersAsync(ct);
        if (string.IsNullOrWhiteSpace(startup))
        {
            await _linuxCommand.WriteIntoConfigFile(new()
            {
                {BaseInfo.PZ_CONFIG_LGSM_STARTPARAMS_KEY, "\"-servername ${selfname}\"" },
            }, BaseInfo.PZ_CONFIG_LGSM_GLOBAL, _safeFileWriter, ct);
            return;
        }
        await _linuxCommand.WriteIntoConfigFile(new()
            {
                { BaseInfo.PZ_CONFIG_LGSM_STARTPARAMS_KEY, $"\"{startup}\"" }
            }, BaseInfo.PZ_CONFIG_LGSM_GLOBAL, _safeFileWriter, ct);
    }

    private async Task EnsureConfigCreated(CancellationToken ct = default)
    {
        var configFileTarget = await _startParamsHelper.GetStartupValueFor(BaseInfo.SERVERNAME_KEY, ct);
        if (configFileTarget == default) return;
        string configFilePath = string.Format(BaseInfo.CONFIG_FILE_PATH, configFileTarget);
        bool configDoesExist = File.Exists(configFilePath);
        if (configDoesExist) return;
        File.Copy(BaseInfo.DEFAULT_CONFIG, configFilePath);

    }
}
